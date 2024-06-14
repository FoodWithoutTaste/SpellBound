using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using OpenAI;

using System.Collections.Generic;
using Cinemachine;
public class ChatGPTClient : MonoBehaviour
{
    private OpenAIApi openAI = new OpenAIApi("sk-proj-FSu2BHaCdcA3Ogbmi0s2T3BlbkFJkAvuYLLj3BN6xRQhCWsA", "org-TdSiFmFWLxFmaZTXAoZqUKh1");
    private List<ChatMessage> messeges = new List<ChatMessage>();
    public async void AskChatGPT(string newText)
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = newText;
        newMessage.Role = "user";
        messeges.Add(newMessage);
        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messeges;
        request.Model = "gpt-3.5-turbo";

        var response = await openAI.CreateChatCompletion(request);
        if(response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponce = response.Choices[0].Message;
            messeges.Add(chatResponce);
            Debug.Log(chatResponce.Content);
        }

    }
}
