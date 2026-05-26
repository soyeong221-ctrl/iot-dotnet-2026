using System;
using System.Collections;   // 컬렉션 네임스페이스

namespace Prac04Collection
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 01. 일반 배열
            // 배열 초기화
            int[] array1 = new int[5];
            array1[0] = 1;
            array1[1] = 2;
            array1[2] = 3;
            array1[3] = 4;
            array1[4] = 5;

            Console.WriteLine(array1);  // python => [1,2,3,4,5] , C# => Int32[]

            // 배열출력
            for (int i = 0; i < array1.Length; i++)
            {
                // for 문으로 배열의 인덱싱
                Console.Write(array1[i] + " ");
            }
            Console.WriteLine(); // 그냥 엔터

            // foreach 사용
            foreach (var item in array1)
            {
                Console.Write(item + ", ");
            }
            Console.WriteLine();

            // 02. 컬렉션
            // 02-1. ArrayList
            ArrayList al1 = new ArrayList();    // 컬렉션은 사이즈 지정 필요없음

            // Add() 메서드로 데이터 추가
            al1.Add(1);
            al1.Add("Hello");
            al1.Add(3.14);
            al1.Add(true);  // Python처럼 타입 제약 없이 입력 가능

            foreach (var item in al1)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("\n");

            al1.Remove("Hello");

            foreach (var item in al1)
            {
                Console.WriteLine(item);
            }

            // ArrayList로 써도 List(Object) 써도 동일
            List<Object> al2 = new List<object>();
            al2.Add(1);
            al2.Add("Hello");
            al2.Add(3.14);
            al2.Add(true);

            // Stack, Queue, HashTable, List .. 

            Hashtable ht1 = new Hashtable();
            ht1["apple"] = "사과";
            ht1["banana"] = "바나나";
            ht1["mango"] = "망고";

            Console.WriteLine(ht1["mango"]);

            Dictionary<String, String> ht2 = new Dictionary<string, string>();
            ht2["apple"] = "사과";

            Console.WriteLine(ht2["apple"]);

        }
    }
}
