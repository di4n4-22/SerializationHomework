using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace DefaultNamespace
{
    [System.Serializable]
    public class PositionSaver : MonoBehaviour
	{
		[System.Serializable]        
        public struct Data
		{
			public Vector3 Position;
			public float Time;
		}
		[SerializeField]
		private TextAsset _json;

        [SerializeField]
        public List<Data> Records { get; private set; }

		private void Awake()
		{
			//todo comment: Что будет, если в теле этого условия не сделать выход из метода?
			//если не сделать выход из метода код продолжит выполняться далее и это приведет к ошибкам
			if (_json == null)
			{
				gameObject.SetActive(false);
				Debug.LogError("Please, create TextAsset and add in field _json");
				return;
			}
			
			JsonUtility.FromJsonOverwrite(_json.text, this);
            //todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
            //позволяет избежать ошибок при работе с пустым списком Records и создает пустой новый список размером 10 если он не был загружен из файла
            if (Records == null)
				Records = new List<Data>(10);
		}

		private void OnDrawGizmos()
		{
			//todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
			//они позволяют избежать ошибок при рисовании линии если список пуст или не инициализирован
			if (Records == null || Records.Count == 0) return;
			var data = Records;
			var prev = data[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
			//todo comment: Почему итерация начинается не с нулевого элемента?
			//чтобы линии рисовались от 1 точки
			for (int i = 1; i < data.Count; i++)
			{
				var curr = data[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}
		
#if UNITY_EDITOR
		[ContextMenu("Create File")]
		private void CreateFile()
		{
            //todo comment: Что происходит в этой строке?
            //создаем файл Path.txt в директории приложения и возвращается объект stream для работы с этим файлом 
            var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
			//todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав) 
			//для освобождения ресурсов
			stream.Dispose();
			UnityEditor.AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
				//Этой командой можно получить путь к ассету через его гуид
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				//Этой командой можно загрузить сам ассет
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
				//todo comment: Для чего нужны эти проверки?
				//для ккорректной загрузки ассета
				if(asset != null && asset.name == "Path")
				{
					_json = asset;
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
					//todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
					//после нахождения нужного ассета нет смысла искать другой
					return;
				}
			}
		}

		private void OnDestroy()
		{
            if (_json == null)
            {
                Debug.LogError("TextAsset _json is not assigned, unable to save records.");
                return;
            }


            var json = JsonUtility.ToJson(this);
            File.WriteAllText(Path.Combine(Application.dataPath, "Path.txt"), json);
        }
    }
#endif
	
}