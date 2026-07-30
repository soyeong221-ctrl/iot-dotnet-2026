using UnityEngine;
using M2MqttUnity;
using System.Collections.Generic;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;
using Newtonsoft.Json;
using TMPro;

// M2MqttClient를 상속한 클래스
public class SmartFactoryMqttClient : M2MqttUnityClient
{
    [Header("Subscribe Topic")]
    public string topic = "smartfactory/52/data";

    [Header("Device ID Text")]
    public TMP_Text txtDeviceId;

    [Header("Timestamp Text")]
    public TMP_Text txtTimestamp;

    [Header("ProductResult Text")]
    public TMP_Text txtData;

    // 감지결과 클래스
    private ProductResult prdResult = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        autoConnect = true;

        base.Start(); // MqMqttUnityClient.start() 실행
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();  // M2MqttUnityClient.update() 계속 실행

        // 데이터가 넘어오면 처리할 로직
        // 컨베이어벨트 스폰, 프로덕트 색상변경...
    }

    /// <summary>
    /// 토픽으로 구독시작 메서드
    /// </summary>
    protected override void SubscribeTopics() {
        // base.SubscribeTopics();  // 부모클래스엔 아무런 로직이 없음

        // 토픽으로 구독시작
        client.Subscribe(
            new string[] { topic },
            new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE }
            );

        Debug.Log($"MQTT Subscribed : {topic}");
    }

    /// <summary>
    /// 구독종료 메서드
    /// </summary>
    protected override void UnsubscribeTopics() {
        // base.UnsubscribeTopics();

        client.Unsubscribe(new string[] { topic });
    }

    /// <summary>
    /// 받은 메시지 디코드메서드
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="message"></param>
    protected override void DecodeMessage(string topic, byte[] message) {
        // base.DecodeMessage(topic, message);

        string msg = Encoding.UTF8.GetString(message);

        //Debug.Log(msg);

        prdResult = JsonConvert.DeserializeObject<ProductResult>(msg);

        Debug.Log(prdResult.deviceId);
        txtDeviceId.text = prdResult.deviceId;
        Debug.Log(prdResult.timestamp);
        txtTimestamp.text = prdResult.timestamp;
        Debug.Log(prdResult.data);
        txtData.text = prdResult.data;        
    }
}
