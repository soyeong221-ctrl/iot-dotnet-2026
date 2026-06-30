using System.Collections.Generic;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ProductApiClient : MonoBehaviour
{
    //[SerializeField]
    //private TMP_Text txtLog;

    [SerializeField]
    // private string serviceUrl = "http://localhost:5065/api/products";   // 개발용 API 주소
    private string serviceUrl = "http://192.168.0.4:8080/api/products";   // 도커 API 주소

    [SerializeField]
    private Transform content;

    [SerializeField]
    private ProductRowUi productRowPrefab;


    public void LoadProudcts()
    {
        StartCoroutine(GetProducts());
    }

    private IEnumerator GetProducts()
    {
        using UnityWebRequest request = UnityWebRequest.Get(serviceUrl);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            //txtLog.text = request.error;
            Debug.LogError(request.error);
            yield break;
        }

        //txtLog.text = request.downloadHandler.text;
        string json = request.downloadHandler.text;

        List<Product> products = JsonConvert.DeserializeObject<List<Product>>(json);


        ClearRows();

        foreach (Product product in products) {

            Debug.Log($"{product.productId}/{product.productName}/{product.price}/{product.stock}");

            ProductRowUi row = Instantiate(productRowPrefab, content);  // content 아래 프리팹 생성
            row.SetData(product);   // 내용 채우기
        }
    }

    private void ClearRows()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
}
