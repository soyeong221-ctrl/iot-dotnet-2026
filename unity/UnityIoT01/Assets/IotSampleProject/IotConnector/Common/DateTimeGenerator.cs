using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IndustryCSE.IoT {
    public class DateTimeGenerator : MonoBehaviour
    {
        [SerializeField] Text _dateTimeText;
        private DateTime _nextLogTime;

        private void Start()
        {
            _nextLogTime = DateTime.Now;
        }

        // 매 프레임마다 발생
        void Update()
        {
            // _nextLogTime 보다 현재 시분초가 더 최신이면
            if (DateTime.Now >= _nextLogTime)
            {
                // 현재 날짜 변경
                DateTime currentDateTime = DateTime.Now;

                // 원본 포맷팅
                // _dateTimeText.text = currentDateTime.ToString("HH:mm ddd dd MMM yyyy");
                _dateTimeText.text = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                // 다음 로그 타임 1분으로
                _nextLogTime = DateTime.Now.AddMinutes(1);
                _nextLogTime = DateTime.Now.AddSeconds(1);
            }
        }
    }
}


