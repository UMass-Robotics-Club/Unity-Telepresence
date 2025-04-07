using UnityEngine;
using Grpc.Core; // For RpcException
using Grpc.Net.Client;
using Cysharp.Net.Http;
using RobotCall; // Make sure this namespace matches your generated code


public class RobotCallClient : MonoBehaviour
{

    [SerializeField] private string serverAddress = "100.79.90.77:50051"; // keep this

    async void Start()
    {
        var httpHandler = new YetAnotherHttpHandler
        {
            Http2Only = true, // Force HTTP/2
            SkipCertificateVerification = true // For development only!
        };
        var channel = GrpcChannel.ForAddress($"http://{serverAddress}", new GrpcChannelOptions
        {
            HttpHandler = httpHandler,
            DisposeHttpClient = true, // Dispose of the HttpClient when the channel is disposed
            MaxReceiveMessageSize = 10 * 1024 * 1024 // 10 MB
        });
        // Check if the channel is created successfully
        if (channel == null)
        {
            Debug.LogError("Failed to create gRPC channel.");
            return;
        }

        // Create client
        var client = new RobotCall.RobotCall.RobotCallClient(channel);

        // Prepare request with sample joint angles
        var request = new RobotCall.RobotCalling
        {
            ShoulderPitchLeft = 45.0f,
            ShoulderPitchRight = -45.0f,
            BicepLeft = 30.0f,
            BicepRight = -30.0f,
            ShoulderRollLeft = 10.0f,
            ShoulderRollRight = -10.0f,
            ForearmRollLeft = 5.0f,
            ForearmRollRight = -5.0f,
            WristFlexionLeft = 15.0f,
            WristFlexionRight = -15.0f,
            WristYawLeft = 20.0f,
            WristYawRight = -20.0f,
            GripperLeft = 0.8f,  // 0=closed, 1=open
            GripperRight = 0.8f
        };

        Debug.Log("Sending robot instructions...");

        try
        {
            // Call RPC
            var response = await client.SendInstructionsAsync(request);

            // Log response
            Debug.Log($"Server response: " +
                      $"Transmitted={response.Transmitted}, " +
                      $"Message=\"{response.Message}\"");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"RPC failed: {ex.Message}");
            Debug.LogError("ptiat");

        }
    }

    
}