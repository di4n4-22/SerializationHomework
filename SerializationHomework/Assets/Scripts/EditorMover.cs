using UnityEngine;

namespace DefaultNamespace
{
	
	[RequireComponent(typeof(PositionSaver))]
	public class EditorMover : MonoBehaviour
	{
		private PositionSaver _save;
		private float _currentDelay;

        //todo comment: Что произойдёт, если _delay > _duration?
        //в проверке в методе Update _currentDelay не достигнет нужного значения и не произведется запись в Records 
        private float _delay = 0.5f;
		private float _duration = 5f;

		private void Start()
		{
            //todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
            //для повышения производительности (чтобы выполнился один раз а не при каждом Update)
            _save = GetComponent<PositionSaver>();
			_save.Records.Clear();

            if (_delay < 0.2f || _delay > 1.0f)
            {
                Debug.LogError("Error: _delay value must be between 0.2 and 1.0");
                _delay = 0.5f; // Set default value if out of range
            }

            if (_duration < 0.2f)
            {
                Debug.LogError("Error: _duration value must be at least 0.2");
                _duration = 1.0f; // Set minimum value
            }
        }

		private void Update()
		{
			_duration -= Time.deltaTime;
			if (_duration <= 0f)
			{
				enabled = false;
				Debug.Log($"<b>{name}</b> finished", this);
				return;
			}

            //todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
            //_delay используется для установки интервала времени между сохранением позиции объекта а не для времени прошедшего с начала выполнения 
            _currentDelay -= Time.deltaTime;
			if (_currentDelay <= 0f)
			{
				_currentDelay = _delay;
				_save.Records.Add(new PositionSaver.Data
				{
					Position = transform.position,
					//todo comment: Для чего сохраняется значение игрового времени?
					//чтобы зафиксировать момент сохранения позицции объекта и воспроизвести движение
					Time = Time.time,
				});
			}
		}
	}
}