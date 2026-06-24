using M2MqttUnity;
using NUnit.Framework;
using System.Text;
using UnityEditor.PackageManager;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;
using TMPro;

public class SmartHomeMqttClient : M2MqttUnityClient
{
    [Header("Subscribe Topic")]
    public string topic = "home/sensor";

    private readonly List<string> receivedMessages = new List<string>();

    protected override void Start()
    {
        brokerAddress = "192.168.0.4";
        brokerPort = 1883;
        autoConnect = true;

        base.Start();   // M2MqttUnityClient의 start() 실행
    }

    protected override void SubscribeTopics()
    {
        // base.SubscribeTopics(); // 부모클래스에 아무런 로직이 없음
        // 토픽으로 구독 시작!
        client.Subscribe(
            new string[] { topic },
            new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE }
            );

        StatusCode.gameObject.SetActive(true);  // 수신중 메시지 출력

        Debug.Log($"MQTT Subscribed : {topic}");
    }

    protected override void UnsubscribeTopics()
    {
        // base.UnsubscribeTopics(); // 부모클래스 virtual 메서드에 아무것도 없음
        // 토픽 구독종료
        client.Unsubscribe(new string[] { topic });
    }

    protected override void DecodeMessage(string topic, byte[] message)
    {
        base.DecodeMessage(topic, message); // 부모클래스에서 로그 출력

        string msg = Encoding.UTF8.GetString(message);

        Debug.Log($"MQTT Topic: {topic}");
        Debug.Log(msg);

        receivedMessages.Add(msg);
    }

    private void Update()
    {
        base.Update();  // 부모클래스는 MQTT 관련 처리 진행

        if (receivedMessages.Count > 0)
        {    // MQTT 메시지가 넘어왔으면

            string msg = receivedMessages[0];
            receivedMessages.RemoveAt(0);


            if (!string.IsNullOrWhiteSpace(msg))
            {
                {
                    msgContent.text = msg; 

                }
            }
        }
