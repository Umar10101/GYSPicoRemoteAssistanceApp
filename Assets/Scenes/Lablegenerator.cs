using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Lablegenerator : MonoBehaviour
{
    public TMP_InputField inputField; // Reference to the InputField where encrypted code is entered

    // Static variables to store the results
    public static string AuthVidLabel;
    public static string AuthAudLabel;

    void Start()
    {
        // Example: Attach the OnProcessButtonClick method to a button's onClick event
        // button.onClick.AddListener(OnProcessButtonClick);
    }

    public void OnProcessButtonClick()
    {
        // Get the input from the InputField
        string encryptedCode = inputField.text;

        // Decrypt the input to get the original 4-digit number
        int decryptedNumber = Decrypt(encryptedCode);

        if (decryptedNumber == -1)
        {
            Debug.LogError("Decryption failed. Ensure input is valid.");
            return;
        }

        // Split the decrypted number into two pairs
        string firstPair = (decryptedNumber / 100).ToString("D2");
        string secondPair = (decryptedNumber % 100).ToString("D2");

        // Add prefixes to create labels
        AuthVidLabel = "10" + firstPair;
        AuthAudLabel = "20" + secondPair;

        // Output the results for debugging
        Debug.Log("AuthVidLabel: " + AuthVidLabel);
        Debug.Log("AuthAudLabel: " + AuthAudLabel);
    }

    // Decrypt function to convert encrypted string back to a 4-digit number
    private int Decrypt(string encryptedCode)
    {
        try
        {
            // Decode base64 string to buffer
            byte[] encryptedData = System.Convert.FromBase64String(encryptedCode);

            // Decrypt using XOR operation with the secret key
            byte[] buffer = new byte[encryptedData.Length];
            for (int i = 0; i < encryptedData.Length; i++)
            {
                buffer[i] = (byte)(encryptedData[i] ^ secretKey[i % secretKey.Length]);
            }

            // Convert buffer back to integer
            int decryptedNumber = System.BitConverter.ToInt32(buffer, 0);

            return decryptedNumber;
        }
        catch (System.Exception)
        {
            // Handle decryption errors
            return -1;
        }
    }

    public static string GetAuthAudLabel()
    {
        return AuthAudLabel;
    }

    public static string GetVideoLabel()
    {
        return AuthVidLabel;
    }

    // Your secret key for decryption (must match the one used in Node.js encryption)
    private byte[] secretKey = System.Text.Encoding.UTF8.GetBytes("mySecretKey123");
}
