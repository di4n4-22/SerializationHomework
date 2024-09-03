using System;
using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(PositionSaver))]
	public class ReplayMover : MonoBehaviour
	{
		private PositionSaver _save;

		private int _index;
		private PositionSaver.Data _prev;
		private float _duration;

		private void Start()
		{
            ////todo comment: зачем нужны эти проверки?
            //метод TryGetComponent проверяет наличие компонента PositionSaver а затем
            //проверяется не пустой ли список Records. далее при некорректных данных 
			//выводится текст ошибки
            if (!TryGetComponent(out _save) || _save.Records.Count == 0)
			{
				Debug.LogError("Records incorrect value", this);
				//todo comment: Для чего выключается этот компонент?
				//для предотвращения некорректного выполнения операций когда данные не соответствуют ожидаемым
				enabled = false;
			}
		}

		private void Update()
		{
			var curr = _save.Records[_index];
			//todo comment: Что проверяет это условие (с какой целью)? 
			//проверяет сколько прошло времени с начала выплнения для перехода к следующим записям
			if (Time.time > curr.Time)
			{
				_prev = curr;
				_index++;
				//todo comment: Для чего нужна эта проверка?
				//для определения дошли ли мы до конца списка записей
				if (_index >= _save.Records.Count)
				{
					enabled = false;
					Debug.Log($"<b>{name}</b> finished", this);
				}
			}
			//todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
			//используются для интерполяции позиции между предыдущей и текущей записями
			//расчет определяет как близко мы находимся к конечной позиции между двумя записями
			var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);
            //todo comment: Зачем нужна эта проверка?
            //проверка обработки случаев деления на ноль или результата NaN  - устанавливается значение 0 для избегания ошибок
            if (float.IsNaN(delta)) delta = 0f;
            //todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
            // применяется линейная интерполяция между предыдущей позицией _prev.Position и текущей curr.Position
            // чем ближе delta к 1 тем ближе мы к конечной позиции по времени
			// рассчитываем текущую позицию объекта между двумя указанными точками
            transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
		}
	}
}