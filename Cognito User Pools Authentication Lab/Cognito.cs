using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using System;
using System.Threading.Tasks;
using Amazon.Extensions.CognitoAuthentication;

public class Cognito : MonoBehaviour
{
    //UI elements 
    public Button SignupButton;
    public Button SignInButton;
    public InputField EmailField;
    public InputField PasswordField;
    public InputField UsernameField;
    public Text StatusText;

    const string PoolID = ""; //insert your Cognito User Pool ID, found under General Settings
    const string AppClientID = ""; //insert App client ID, found under App Client Settings
    static Amazon.RegionEndpoint Region = Amazon.RegionEndpoint.USEast1; //insert region user pool was created in 

    bool signInSuccessful;


    void Start()
    {
        SignupButton.onClick.AddListener(on_signup_click);
        SignInButton.onClick.AddListener(on_signin_click);
        signInSuccessful = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (signInSuccessful)
            SceneManager.LoadScene(1);
    }

    public void on_signup_click()
    {
        Debug.Log("sign up button clicked");
        _ = SignUpMethodAsync();
    }

    public void on_signin_click()
    {
        Debug.Log("sign in button clicked");
        _ = SignInUser();
    }

    //Method that creates a new Cognito user
    private async Task SignUpMethodAsync()
    {

        Debug.Log("Signup method called");

        string userName = UsernameField.text;
        string password = PasswordField.text;
        string email = EmailField.text;

        AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), Region);

        SignUpRequest signUpRequest = new SignUpRequest()
        {
            ClientId = AppClientID,
            Username = userName,
            Password = password
        };

        List<AttributeType> attributes = new List<AttributeType>()
            {
                new AttributeType(){Name = "email", Value = email}
            };

        signUpRequest.UserAttributes = attributes;

        try
        {
            SignUpResponse request = await provider.SignUpAsync(signUpRequest);

            Debug.Log("Sign up worked");
        }
        catch (Exception e)
        {

            Debug.Log("EXCEPTION" + e);
            return;
        }

    }

    //Method that signs in Cognito user 
    private async Task SignInUser()
    {
        string userName = UsernameField.text;
        string password = PasswordField.text;
        string email = EmailField.text;

        AmazonCognitoIdentityProviderClient provider = new AmazonCognitoIdentityProviderClient(new Amazon.Runtime.AnonymousAWSCredentials(), Region);

        CognitoUserPool userPool = new CognitoUserPool(PoolID, AppClientID, provider);

        CognitoUser user = new CognitoUser(userName, AppClientID, userPool, provider);

        InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
        {
            Password = password
        };

        AuthFlowResponse authResponse = null;
        try
        {
            authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);

            GetUserRequest getUserRequest = new GetUserRequest();
            getUserRequest.AccessToken = authResponse.AuthenticationResult.AccessToken;

            Debug.Log("User Access Token: " + getUserRequest.AccessToken);
            signInSuccessful = true;
        }
        catch(Exception e)
        {
            Debug.Log("EXCEPTION" + e);
            return;
        }

       

    }






}