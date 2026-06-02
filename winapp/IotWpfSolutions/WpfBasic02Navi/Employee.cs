using System;
using System.Collections.Generic;
using System.Text;

namespace WpfBasic02Navi
{
    public class Employee
    {
        public int Id { get; set; }  // PK와 동일

        public string Name { get; set; }  // 직원명

        public string Department { get; set; }  // 부서명

        public int Salary { get; set; } // 급여

        public DateTime HireDate { get; set; } // 입사일

        public bool IsActive { get; set; } // 활동여부
    }
}