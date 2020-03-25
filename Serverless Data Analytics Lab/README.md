![AWS_logo_RGB.png](http://d2a4jpfnohww2y.cloudfront.net/cognito/AWS_logo_RGB.png)

<a id="Top"></a>
## Serverless Data Analytics Lab
This lab will guide you through creating a serverless analytics pipeline and integrating it into a Unity game. This lab will focus on using serverless services to build an analytics pipeline for both batch and near real-time analytics. This way, you can get batch insights over a period of time and also see data populating on a graph in near real-time. Services used in this lab include: Amazon API Gateway, AWS Lambda, Amazon Kinesis, Amazon S3, AWS Glue, Amazon Athena, Amazon QuickSight, and Amazon CloudWatch. In this lab, you will build out the following architecture:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/analytics3.png" /></p> 

## Agenda 

* [Overview](#Overview)
* [Task 1: Setting up prerequisites and permissions](#TaskPre)
* [Task 2: Setting up Amazon API Gateway and an AWS Lambda backend](#TaskAPI)
* [Task 3: Creating an Amazon S3 data lake and Amazon Kinesis Firehose stream for data ingestion](#TaskS3)
* [Task 4: Integrating AWS with Unity](#TaskUnity)
* [Task 5: Populating data lake with Amazon Kinesis Data Generator](#TaskKinesis)
* [Task 6: Using AWS Glue to discover data](#TaskGlue)
* [Task 7: Querying data with Amazon Athena](#TaskAthena)
* [Task 8: Discovering batch insights with Amazon QuickSight](#TaskQuicksight)
* [Task 9: Setting up a near real-time pipeline with Amazon Kinesis Analytics, AWS Lambda, and Amazon CloudWatch](#TaskRealTime)
* [Clean up](#cleanup)
* [Appendix - additional reading](#additionalreading)


<a id="Overview"></a>
[[Top](#Top)]

## Overview

### Why use analytics in my game?

Using analytics is very important to improve your game and keep players around. Incorporating an analytics pipeline to your game can help you create more engaging games by doing data driven game development. You can learn how to optimize the game play experience so you can attract more players and increase player engagement. It can help with anomaly detection by identifying abusers, cheaters, and player churn. It can also help you improve your game infrastructure by better understanding peak usage times so you know when to scale. Finally, data analytics can help with revenue generation by encouraging purchases, targeted ads, and content recommendations. There are many use cases for analytics in games, and if you would like to learn more check out the [Why Analytics For Games AWS Training Course](https://www.aws.training/Details/eLearning?id=42751)


### Serverless analytics pipeline on AWS

Serverless applications don’t require you to provision, scale, or manage any servers. You can build them for nearly any type of application or backend service, and everything required to run and scale your application with high availability is handled for you. A lot of companies are making the move towards serverless, even for their analytics workloads. AWS has an expansive ecosystem of services that can be used to build out a robust analytics pipeline for your game so that you can discover useful insights on player and game data. Serverless is a great option for a lot of games companies because many of them do not have dedicated resources or a data analytics expert on hand to configure infrastructure and manage a data analytics pipeline. 

##### Batch and near real-time analytics 

Depending on the questions you have and the answers you are looking to get, you might be required to analyze data at different speeds. It is important to understand that data has a shelf-life. The older data becomes, the less useful it is in helping you with timely reactions. To derive accurate insights, consider the type of answers you want. Align your questions to the speed with which you gather and analyze data.

For example, consider if you made recent enhancements to your game like a new weapon or downloadable content (DLC), such as a new game level. You want to quickly assess if players react positively. On the other hand, metrics such as daily active users (DAU) or monthly active users (MAU) must draw from data that is compiled over a day or calendar month.

This is why there are primarily two types of analytics pipelines you want to focus on: batch and near real-time. Batch analytics is analyzing data collected over a period of time, where near real-time analytics is analyzing data as it comes in. 

This lab covers building out a robust pipeline that includes both batch and near real-time analytics. Some services that you can use on AWS for analytics include:

##### Amazon S3

Amazon S3 provides durable object storage in the AWS cloud. It makes for a great data lake solution because of its virtually unlimited scalability. As your data grows, you can increase storage from gigabytes to petabytes of data. It allows you to decouple storage and compute so that you can scale both independently as needed. This can act as a central data store for all of your game data. 

##### Amazon Kinesis 

Amazon Kinesis makes it easy to collect, process, and analyze real-time streaming data so you can get timely insights and react quickly to new information. You can use Kinesis for real-time delivery of in-game data collected from game servers and clients to be stored in your S3 data lake. There are multiple different flavors of Amazon Kinesis, but two will be covered in detail in this lab: Kinesis Firehose, which allows you to easily deliver streaming data to 4 built-in destinations on AWS including Amazon S3, and Kinesis Analytics, which allows you to run SQL queries on your streaming data in real-time. 

##### AWS Glue

AWS Glue is a fully managed extract, transform, and load (ETL) service that makes it easy to prepare and load data for analytics. You can use AWS Glue to discover the data in your S3 data lake to make it searchable, queryable, and available for ETL. 

##### Amazon Athena

Once your data has been discovered using AWS Glue, you can use Amazon Athena to query your data using standard SQL. With Athena, you can query exactly the data you want to analyze. 

##### Amazon QuickSight

Once you query the data you are interested in analyzing, you can use Amazon QuickSight as a business intelligence service to discover insights about your game data. QuickSight works really well for batch analytics with data that has been collected over a time frame of a couple hours, to days, months, even years. You can create and publish interactive dashboards and visualizations. You can even discover hidden trends and do forecasting using machine learning. With QuickSight, you can answer questions about your game - is it too hard? Is it too easy? Are your players engaged and going to stick around? 

##### AWS Lambda 

AWS Lambda is serverless compute capacity in the cloud, which lets you run code without provisioning or managing servers. Lambda will be used as an orchestration tool in this pipeline to execute code that can send data from your game to your AWS analytics pipeline. 

##### Amazon CloudWatch

Amazon CloudWatch is a monitoring and observability service that provides you with data and actionable insights to monitor your applications, respond to system-wide performance changes, optimize resource utilization, and more. You can instrument your pipeline to send custom metrics to CloudWatch and view these metrics on an automatic dashboard that CloudWatch provides. This service works well for visualizing data in real-time. 

### Unity

Unity is a cross-platform game engine developed by Unity Technologies that is used to create the core game that will be the basis of this lab. 

### Getting Started

This lab will focus specifically on building out a serverless analytics pipeline and integrating it into a Unity game. You will first set up an API Gateway and a Lambda function to send data from your game to your analytics pipeline. Then, you will create an S3 data lake and populate it with Kinesis, discover the data using Glue, practice querying the data with Athena, and then create batch visualizations from the data with QuickSight. After that, you will use Kinesis Analytics to run SQL queries on your streaming data in near real-time, and view that data as it comes in on a CloudWatch dashboard. 
 
 <a id="TaskPre"></a>
[[Top](#Top)]

## Task 1: Setting up prerequisites and permissions
 
 ### Prerequisites

For the purposes of this lab, you will be using the AWS Management Console as well as Unity. You will need to have:

* An **AWS account** with an appropriate level of permissions to use the services needed for this lab (S3, Lambda, Kinesis, Glue, Athena, QuickSight, CloudWatch, API Gateway). Follow the link to create and activate a new AWS account if you do not have one already: https://aws.amazon.com/premiumsupport/knowledge-center/create-and-activate-aws-account/
* **Unity 2019.1.0** - Download Unity and Unity Hub from this link: https://unity3d.com/get-unity/download/archive

This lab works for both Mac and Windows. If you already have these prerequisites, you can skip to [[Task2](#Task2)]

### Setting up permissions

First, you will need to create an IAM user with the appropriate permissions needed to do the lab if you do not have one already. AWS Identity and Access Management (IAM) enables you to manage access to AWS services and resources securely. Using IAM, you can create and manage AWS users and groups, and use permissions to allow and deny their access to AWS resources. It is highly recommended that you do not use the default root user of your AWS account and instead provision your own IAM user for security purposes.

1. Sign into your AWS account and go to the IAM landing page by clicking **Services > IAM** or by clicking this [quick link](https://console.aws.amazon.com/iam/).

2. Click **Users** on the left-hand navigation pane and then select **Add user**.

3. Give your user a user name and enable **AWS Management Console access** so that you can give your user the ability to sign-in to the AWS Management console. You can optionally choose to enable **Programmatic access** so that you can download an access key and secret access key to use the AWS Command Line Interface (CLI). It is not needed for this lab, but it is a good tool to use.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/IAM1.png" /></p> 

4. Select **Next: Permissions** and choose **Attach existing policies directly**. Choose the AdministratorAccess policy to add to your user.

**SECURITY DISCLAIMER:** Here, you are adding full administrator access for simplicity of lab purposes. However, it is best practices that with IAM you assign fine-grained permissions to AWS services and to your resources. If you want to make your permissions more fine-grained and not use admin permissions, you can add permissions for only the services that will be used in this lab, including API Gateway, Lambda, S3, Kinesis, Glue, Athena, QuickSight, and CloudWatch. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/IAM2.png" /></p> 

5. Select **Next: Tags**, **Next: Review** and finally **Create user**.

6. You can **Download .csv** to download and save your access key ID and secret access key for use later when setting up the CLI optionally - again, this is not needed for this lab but it is a good tool to use in the future when working with AWS if you prefer to work from the command line.

7. Sign into the AWS Management Console with the IAM credentials you just created. 

You are done setting up the prerequisites needed for this lab.

 
<a id="TaskAPI"></a>
[[Top](#Top)]

## Task 2: Setting up Amazon API Gateway and an AWS Lambda backend

The first step is to configure an Amazon API Gateway and an AWS Lambda function. API Gateway is a fully managed service that makes it easy for developers to create, publish, maintain, monitor, and secure APIs at any scale. It will act as the "front door" for your analytics pipeline and provide an extra layer of security for your backend resources so you do not have to bake AWS credentials into a game client, which poses a security risk. Lambda lets you run code in a serverless fashion, so it will act as a backend orchestration service for sending data from your game to your analytics pipeline. This pattern provides an additional layer of security by abstracting all API calls away from the player and instead making them in a backend. It also makes your implementation less brittle, because if API calls are made in the client instead of a backend you would have to push updates to your players each time you want to change something. 

1. Sign into the **AWS Management Console** and on the Services menu, click **IAM** or use this [quick link](https://console.aws.amazon.com/iam/). We need to configure an Identity and Access Management role first to define appropriate permissions for your Lambda function. 

2. On the left-hand navigation pane, click **Roles**. 

3. Click **Create role**.

4. For _Select type of trusted identity_, under AWS Service, choose **Lambda**.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/trusted.png" /></p> 

5. Click **Next: Permissions**.

6. In the box where you can filter permissions policies, type in **KinesisFirehose** and select the box next to **AmazonKinesisFirehoseFullAccess**.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/iamkinesis.png" /></p> 

7. Then click **Next: Tags** and **Next: Review**. 

8. Give the role a recognizable name, this lab uses **KinesisProducerLambdaRole** for example. You can also optionally provide a description.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/iamcreaterole.png" /></p> 

9. Finally, click **Create role**. 

10. Now, let's create the Lambda backend. In the Management Console, select **Lambda** or use this [quick link](https://console.aws.amazon.com/lambda/).

11. Click **Create function** and select **Author from scratch** which should be chosen by default.

12. Name your function **KinesisProducer** and choose **Python 3.8** as the runtime.

13. Under permissions, select **Choose or create an execution role** 

14. Select **Use an existing role** and pick the one you just created. 

15. Finally, click **Create Function**. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/lambdaconfig.png" /></p> 

16. Create an **environment variable** that will contain your Kinesis Firehose stream name that you will create in the next step. Set the key as **deliveryStreamName** and the value as **serverless-games-stream**. It is important that you stay consistent with the naming convention. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/environmentvariable.png" /></p> 

17. Copy and paste the following code into the body of the Lambda function (can also be found in the GitHub repository named **Kinesis_Lambda.py**):

```
import json, os, boto3

def lambda_handler(event, context):
    
    #get delivery stream name from environment variable
    deliveryStreamName = os.environ['deliveryStreamName']
    
    #create delivery stream client
    client = boto3.client('firehose')
    
    #encode the json string as Kinesis expects bytes
    data = event['body'].encode()
    
    #put record to delivery stream
    response = client.put_record(
        DeliveryStreamName = deliveryStreamName,
        Record={
        'Data': data
        }
    )
    
    return response
```

* This code imports Boto 3 which is the AWS SDK for Python. Using the SDK for Kinesis, it initializes a Kinesis Firehose delivery stream client. It then uses a function to encode the JSON payload that is emitted from the game as bytes, which is the data type that Kinesis expects. Finally, it creates a _put_record_ request that specifies the Kinesis delivery stream name and the data to be sent to that Kinesis delivery stream.

18. Click **Save** to save your Lambda function. 

19. Now it is time to set up the API Gateway. In the AWS Management Console, under Services, select **API Gateway** or use this [quick link](https://console.aws.amazon.com/apigateway/).

20. In the upper right hand corner, click **Create API**

21. Choose the **HTTP API** type and click **Build**. This is a more cost effective option to use if you just want to support simple HTTP operations. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/apitype.png" /></p> 

22. For **Integration type**, add an integration and choose **Lambda**. Select the region you are working in and choose your lambda function. 

23. Give your API Gateway a name, for example _serverless-games-analytics_. Your configurations should look like the following:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/apiname.png" /></p> 

24. Click **Next**.

25. You need to create a route that will target this Lambda function. You can use _ANY_ as a catch-all. In this example we are just supporting _POST_. Set the **Method** as _POST_ and then click **Next**. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/postt.png" /></p>

26. On the Define stages page, accept all default configurations and click **Next**. 

27. Click **Create** and your API Gateway will automatically deploy. 

Congratulations! You set up your API Gateway and your Lambda backend! Now to move onto creating the analytics pipeline.

<a id="TaskS3"></a>
[[Top](#Top)]

## Task 3: Creating an Amazon S3 data lake and Kinesis Firehose stream for data ingestion 

The next step is to set up your data storage and your ingestion mechanism. For your data storage, you are going to use Amazon S3, which will act as your centralized data lake for all your game data. 

1. In the **AWS Management Console** on the Services menu, click **S3** or use this [quick link](https://console.aws.amazon.com/s3/).
2. Click **+ Create bucket**.
3. Enter a bucket name. It has to be globally unique across all existing buckets in S3. This lab will use a bucket named _serverless-games_.
4. Choose the region for this bucket. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/1.png" /></p> 

5. Click **Create**.

You have created your S3 data lake! Now you need to set up an ingestion mechanism so you can stream data from your game to your S3 data lake. You can do this using an Amazon Kinesis Data Firehose stream. Kinesis Firehose is scalable so it allows you to ingest records from many clients simultaneously. It can stream data to multiple destinations besides just S3 and it integrates easily with other AWS services like Kinesis Data Analytics to process your streaming data using standard SQL, which will be set up later in this lab. 

6. In the AWS Management Console, go to Services and click **Kinesis**.

7. In the top right corner, you will see a section for Kinesis Firehose delivery streams. Click **Create delivery stream**.

8. Give your stream the name _serverless-games-stream_. It is important you use this name because it is what you set as the envirornment variable when you configured the Lambda in the last step. 

9. Under Choose source, keep it the default as _Direct PUT or other sources_.

10. Hit **Next**.

11. Leave these configurations as default and hit **Next** again.

12. Under Select destination, make sure **Amazon S3** is selected.

13. On the same page under S3 destination, choose the S3 bucket you created previously. 

14. You can take a look at all the other configuration options and explore them if you want but for now leave them all default and hit **Next**.

16. Kinesis Firehose buffers incoming records before delivering them to your S3 bucket. Set **Buffer size*** to 1 MB and **Buffer interval** to 60 seconds. This is to make sure data is delivered to S3 as quick as possible.  

17. Scroll down to IAM role. This is the Identity and Access Manamgenent role that you need to specify to give Kinesis the appropriate permissions it needs to access your S3 bucket and any other resources it may need. Click **Create new or choose**.

18. This will open up a new tab like the one below where you can create a new IAM role. Select **Allow**.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/10.png" /></p> 

19. You should be redirected back to the tab where you are configuring your Kinesis Firehose stream. Review your configuration settings and finally select **Create delivery stream**. You should now see your newly created stream on the Kinesis Firehose dashboard. It will take a minute or two to create, but once it says the status is active you can click into it to find details. 

<a id="TaskUnity"></a>
[[Top](#Top)]

## Task 4: Integrating AWS with Unity

Now that you have your API Gateway set up, your data storage in place, and your ingestion mechanism, it is time to create a sample project in Unity that you will begin integrating your analytics pipeline with. 

1. **Click** the 3D Beginner Complete Project from the following link: https://learn.unity.com/project/john-lemon-s-haunted-jaunt-3d-beginner and find the **Project Materials**, as shown in the screenshot below. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/cognito/unity1.png" /></p> 

 * You will use a sample game that has been built already for you in Unity. You will take this core game and add functionality to it using AWS as you progress through this lab. The baseline of this game is built using the free John Lemon’s Haunted Jaunt assets from the Unity Asset store at the link in the previous step. You can run through the tutorial yourself if you wish, which takes around 5 and a half hours, or you can simply download the completed sample project. For this tutorial, it is assumed that you download the completed sample project. 
 
2. When you click the link above, it should open up the Unity Asset Store. Click **Add to My Assets** and sign into your Unity account if necessary. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/cognito/unity2.png" /></p> 

3. Once you do this, you should see an option to open these assets in Unity. Click **Open in Unity**. This will open Unity Hub. 

4. On the left navigation panel in the Unity Hub, click **Learn**. This will open up a page where you can find sample projects to download. You should see the John Lemon's Haunted Jaunt: 3D Beginner project downloaded. **Click** this to open the project in Unity.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/cognito/unity4.png" /></p> 

5. **Download** the Unity package in this GitHub repository called _ServerlessAnalytics.unitypackage_. **Import** this package into your Unity game by choosing Assets > Import Package > Custom Package.

	* This game has already been built for you. Browse the different assets folders – take a look at the Scripts, Prefabs, Animation folders and more to see what gives the game its functionality. 
	
6. Click on the **Scenes** folder under Assets. Open the scene titled **MainScene**. This is the scene where you can play the actual game - try playing it if you want! The goal is to escape from the haunted house while avoiding enemies. 

7. You need to fix the build settings for this project before getting started. Under File > Build Settings, click **Add Open Scenes**. It should add the MainScene under index 0. Your final configurations should look like this:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/buildsettings.png" /></p> 

8. **Do not click** Build or Build and Run, simply exit out of the Build Settings window.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/kinesissdk.png" /></p> 
    
Now that you have your Unity sample game open and you have explored around a bit, it is time to begin coding some AWS functionality into the game. You will add code that will be able to post data to API Gateway, which will trigger the Lambda function to send the data to your Kinesis stream to store it in your S3 data lake. 

   * **Note:** There are a couple different ways to integrate AWS into your game depending on your use case. When doing this, we recommend minimizing the number of AWS resources accessed directly from your game client. It introduces a security risk when you expose a lot of API calls from your client and it also makes your implementation brittle, forcing you to create client updates every time you need to make a change. 
   
   * It is recommended to use a “front door” to your AWS resources, like Amazon API Gateway for example. You can making authenticated calls to send and retrieve data from API Gateway. You can authenticate calls using Lambda authorizers or Cognito authorizers and make SDK calls from a backend, which could be a server, a Lambda function, or even containers. For example, you can emit game events with a POST to your API Gateway from a game client. This provides an extra layer of security, but also is an additional cost. 
   
   * To read more integrating AWS into a game, check out this blog post: https://aws.amazon.com/blogs/gametech/game-developers-guide-to-the-aws-sdk/
   
9. In the object heirarchy, you will see an Analytics parent object which has a PutData child object. 

10. Navigate to the **Scripts** folder in Assets and open up the **PutData.cs** script. This script is attached to the PutDatta child object. Explore this script. 

* This is the file that gathers game data when a player successfully escapes or dies from the haunted mansion, and makes a POST to API Gateway to send that data to the analytics pipeline. This is the code in the script that makes the POST request to API Gateway using Unity's built-in _UnityWebRequest_ which handles the flow of HTTP communication. There are no external modules required! 

* **Note:** The WWWForm api is eventually planned for deprecation, those looking to future proof will want to use List<IMultipartFormSection>. 

```
    IEnumerator Post(string url, string jsonData)
    {
        UnityWebRequest uwr = UnityWebRequest.Post(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Content-Type", "application/json");
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }
    }
```

* The data that is being emitted from the game is a JSON payload that contains the player ID, time played, wins, and losses. This is gathered in the **DataPoint** class. 

11. Close the script. 

12. **MainScene** should already be open, but if it's not, go to Assets > Scenes > and open MainScene. It is time to begin playing the game to test it out.

13. Hit **play**. The goal is to avoid enemies and escape from the haunted house. Play around a bit - lose a couple times and try to win if you want. This will send some sample data to your API Gateway with a POST which will trigger the Lambda function to send it to your Kinesis stream so that the data ends up in your S3 bucket. Don't worry about trying to send a lot of data now, you will ingest a lot of sample data to your bucket in the next step. 

14. Stop playing the game and monitor how your Kinesis Firehose stream is performing. Go to the **AWS Management Console**, click **Kinesis** and find your **Kinesis Firehose delivery stream**. 

15. Click into your stream to see details about it and select the **Monitoring** tab. You can see Amazon CloudWatch metrics, similar to the ones shown below. These metrics show the amount of incoming records, the amount of records successfully delivered to S3, and more. Data might not be immediately visible on these graph due to the buffer interval of your stream. If you do not see data immediately, wait a few minutes and refresh. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/kinesis7.png" /></p>

16. In the **AWS Management Console**, go to **S3** and find the S3 bucket you created earlier in this lab. Look at the contents of this S3 bucket. If you drill down into the folders, you should see data in there that looks similar to this:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/kinesis8.png" /></p>

17. **Download** one of the files by clicking on it and hitting download. Take a look at the contents. You should see your game data in JSON format like below:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/kinesis9.png" /></p>

Congratulations! You successfully created an S3 data lake, a Kinesis Firehose stream, and integrated it with a Unity game.

<a id="TaskKinesis"></a>
[[Top](#Top)]

## Task 5: Populating data lake with Amazon Kinesis Data Generator

Now you are able to successfully send your own player data to S3 as you play the game. Right now, this is a small amount of data since you are only one player. You want to simulate this on a larger scale with more data so you can see useful visualizations. For the purpose of this lab, you will use the Kinesis Data Generator to simulate data. 

1. The **Amazon Kinesis Data Generator** is found at this link: https://awslabs.github.io/amazon-kinesis-data-generator/web/producer.html. Open this link - you should see a webpage similar to the one below. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/11.png" /></p> 

2. Click the **Help** button. This page will walk you through how to configure Kinesis Data Generator with your AWS account. Follow the steps to **Create a Cognito User with CloudFormation**.

3. This will bring you to the AWS Management Console CloudFormation dashboard. You should see the followng:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/12.png" /></p> 

4. Hit **Next**. Create a username and password that will be used to sign into the Kinesis Data Generator. Hit **Next** again.

5. Leave all other configuration default and hit **Next** twice more. 

6. Review your configurations and check the box at the bottom that acknowledges that this CloudFormation template might create IAM sources. Finally click **Create stack**. It will take a few minutes for the CloudFormation stack to finish creating.

7. When it is finished, on the _Outputs_ page of the stack you will see a **KinesisDataGeneratorUrl**. Open this link. You will use this link to sign in to a page that looked the same before. Sign in using the username and password you just configured in the CloudFormation template. 

8. When the log-in is successful, you will see some fields that you can start configuring to send sample data to populate your S3 data lake. 

      * Set the **Region** as the same one you created your Kinesis Firehose stream in.
      * Set the **Stream** as the one you created in Task 2.
      * On the **Record template** under Schema Discovery Payload, put the following:
 
```
{
"PlayerID":{{random.number(10000)}},
"Time Played":{{random.number(100000)}},
"Wins":{{random.number(5)}},
"Losses":{{random.number(50)}},
"CaughtAt":{{random.weightedArrayElement(
{
    "weights": [0.2,0.5,0.1,0.05,0.1,0.05],
    "data": [1,2,3,4,5,6]
  }
)}},
"CaughtBy":{{random.weightedArrayElement(
{
    "weights": [0.2,0.45,0.15,0.05,0.1,0.05],
    "data": [1,2,3,4,5,6]
  }
)}}
}
```

This data represents random players playing your game. For the _CaughtAt_ variable, the options are [1,2,3,4,5,6] which correspond to different parts of the haunted mansion. For example, let's say that 1 is the living room, 2 is the hallway, 3 is the bathroom, 4 is the kitchen, 5 is the basement, and 6 is the final room before the escape. For the _CaughtBy_ variable, the options are also [1,2,3,4,5,6]. Let's say that numbers 1-3 represent different ghosts in the house, while 4-6 represent different gargoyles. Your final configurations should look similar to this:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/KDG.png" /></p> 

9. Hit **Send data**. You should see data starting to send. Let the data generator send a couple thousand records (3000 records for example would be a good stopping point) and then finally hit **Stop sending data to Kinesis**. **Leave this tab open because you will need to send more data later.**

10. Go back to the AWS Management Console tab with your Kinesis Firehose delivery stream open. Click into the stream to view the stream details if you are not viewing them already.

11. Select the **Monitoring** tab to view metrics like you did in Task 2. You should see something similar to what is shown below to verify the Kinesis Data Generator is working:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/14.png" /></p> 

12. Go back to your S3 bucket to look at the contents and **verify** the data has been delivered. 

Now you have a lot of sample player data to work with for the rest of the lab. 


<a id="TaskGlue"></a>
[[Top](#Top)]

## Task 6: Using AWS Glue to discover data

Now that you have all of the data you want to analyze in your S3 data lake, it is time to discover that data and make it available to be queried. 

1. In the AWS Management Console, go to Services, and click **AWS Glue**. 

2. On the left-side navigation bar, select **Crawlers**. You are creating a Glue Crawler, which will _crawl_ through the data in your S3 bucket. It is going to connect to your S3 data store and classify it to determine the schema and metadata. 

3. Select **Add crawler**.

4. Enter a crawler name. For this lab, _serverless-games-crawler_ will be used as the name.

5. Select **Next**. Leave the crawler source type as default, which should be Data stores. Select **Next**.

6. Now, you choose the data store you want to crawl through. It should be defaulted to S3, but you still need to specify the bucket and path of the data you want to discover. Select the **folder icon** to navigate to the path that the Kinesis Data Generator data is at.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/15.png" /></p> 

7. Find your bucket, click the **+** next to it, and keep navigating inside the sub folders until you find the file that contains the sample data. Select the folder that file is in, similar to this:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/16.png" /></p> 

8. Hit **Select** and then **Next**. Do not add another data store - continue hitting **Next**

9. As you did when you created the Kinesis Data Firehose stream, you need to create an IAM role for your Glue crawler to allow it permissions to access resources it might need access to. Create a role, give it a name, and select **Next**.

10. Keep the Frequency to run on demand and select **Next**.

11. On the page where you define a database, select **Add database** and give it a name. This lab will use the name _serverless-catalog_. Hit **Create** and then **Next**.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/17.png" /></p> 

* You just created a Glue Data Catalog, which contains references to your data in S3. It is an index to the location, schema, and runtime metrics of your data and is populated by the Glue crawler. 

12. Review your configurations and select **Finish** to create the crawler. 

13. You should be redirected to AWS Glue dashboard. Find the crawler you just created, select it, and hit **Run crawler**. It might take a few minutes for your crawler to run, but when it is done it should say that a table has been added. Wait for your crawler to finish running.

14. On the left-side navigation bar, select **Databases**. You should see the Glue Data Catalog that you have created. Select it and then click the link to view the tables in your catalog. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/18.png" /></p> 

* You should see a table if you click into it you will see information about the table and the schema of it like below:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/glue.png" /></p> 

Congratulations! You have successfully used AWS Glue to create a crawler and populate a Glue Data Catalog to discover the data in your S3 data lake.


<a id="TaskAthena"></a>
[[Top](#Top)]

## Task 7: Querying data with Amazon Athena

Now that you have made your data discoverable, you can query your data for exactly what you want to analyze. 

1. Go to AWS Management Console, click Services, then select **Athena**.

2. On the left hand side under Database, select the **Glue Data Catalog** you just created. In the case of the lab, it is the _serverless-catalog_.

3. Create a new **query** in the middle pane of the Athena Dashboard. Here you can write standard SQL to run queries against your Glue Data Catalog. Run the following query:

```
SELECT * FROM "serverless-catalog"."23" WHERE "serverless-catalog"."23"."time played" > 3600
```

* You might need to change where it says _serverless-catalog_ to the name of your own Glue Data Catalog. You also might need to change the table name to match the name of yours, which you can find in the left-hand "Database" pane. For reference, the table name here is "23" so change this to match your table name. Here we are querying all the data where the players had spent more than 3600 seconds, or 1 hour, playing the game. 

* You also might need to set up an S3 bucket destination where the output of your queries get saved. If "Run Query" is greyed out, there will be a link at the top of the screen prompting you to specify an S3 destination. You can also edit your S3 destination by clicking "Settings". 


* Your final result should look like this:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/athena.png" /></p> 

* If you are recieving zero results returned from your queries, make sure each file in your S3 bucket is in its own individual folder. Otherwise, if there are multiple files in the same folder when using Glue, Athena will return zero records. Re-structure your S3 bucket and re-run the Glue crawler to fix this issue. 

4. After you run the query and the results display on the dashboard, click **Create** and then **Create view from query**. Give it a name. This lab uses _data-view_ as the name. Click **Create**. 

Congratulations! You have finished querying your data using Amazon Athena. 

<a id="TaskQuicksight"></a>
[[Top](#Top)]

## Task 8: Discovering batch insights with Amazon QuickSight

Now that you have queried for a subset of your data, it is time to analyze it using Amazon QuickSight.

1. Go to the AWS Management Console, select Services, and then choose **QuickSight**.

2. Before importing your data and creating visualizations, you need to make sure that certain permissions are in order. Change the **region** in the top right corner to US East (N. Virginia) as this is the only region that you can currently edit QuickSight settings in. 

3. In the top right, next to where you just changed the region, select your user and then choose **Manage QuickSight** from the drop down. 

4. On the left-hand panel, select **Security & permissions**. You need to allow QuickSight access to certain AWS services. Click **Add or remove** to edit the services QuickSight has permissions to access. Make sure both S3 and Athena are enabled. It is important to **uncheck** and then **recheck** the box for S3 to ensure your newly created bucket is included. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/22.png" /></p> 

5. After ensuring the correct permissions, go back to the region you were working in before if it is not US East 1, since this is the region you have been working in the entire lab and will be where your Glue Data Catalog and your Athena table view are. 

6. Once you are on the QuickSight dashboard, select **New analysis** in the top left corner. 

7. Select **New data set** and then click **Athena**. You are going to use the table view you just created as the data set for your QuickSight visualizations, to only analyze player data for those who played more than 1 hour. 

8. Enter a name for your data source. This lab will use _serverless-games-data-source_ as the name. Click **Create data source**.

9. Next, it says to choose your table. Select the **Glue Data Catalog** you queried in Athena. Then, select the **table view** you just created using Athena in the last task. It should look similar to this:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/21.png" /></p> 

10. Hit **Select**. Next, you can choose to **Import to SPICE for quicker analytics** if you want to do so. SPICE is QuickSights in-memory calculation engine that improves the performance of importing data and creating visualizations. Select **Visualize**.

* If you are running into a permissions error, similar to something like "An error has been thrown from the AWS Athena client. Access Denied (Service: Amazon S3)", then go back to step 4 and make sure you are including your newest S3 bucket by **unchecking** and then **rechecking** the box for S3.

11. Once your data has been imported, you can start playing around with QuickSight to see what visualizations you can create. Select the **+ ADD** button on the top left to add a visualization. 

12. Under Visual types, select a **Vertical bar chart** visualization. 

13. Drag the **caughtby** variable to be the value for the X axis.

14. Your graph should look similar to this: 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/caughtby.png" /></p> 

* You can change the colors to represent the different enemies. Remember that numbers 1-3 correspond to ghosts while 4-6 correspond to Gargoyles. In this picture, blue is gargoyles while purple is ghosts. This graph shows the number of players caught by each individual enemy. We can see that players die by ghost #2 the most. This can indicate that either the other enemies are catching the player enough and the game is too easy, or on the opposite end of the spectrum that players are having a hard time getting past ghost #2 and barely winning the game. 

15. Next, let's create a heat map thats where players are dying the most. Select the **+ ADD** button on the top left to add a new visualization.

16. Under Visual types, select a **Heat map** visualization. 

17. Drag the **caughtat** variable to be the value for Rows and the **caughtby** variable to be the value for Columns.

18. Your graph should look similar to this: 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/heatmap.png" /></p> 

* This heat map shows where players are caught at verses what enemy players are caught by. This heat map shows a pattern that most players are caught by ghost #2 and most players die in the hallway. If we look at the way our game is structured, ghost #2 is in the hallway. This graph further indicates that our level design may be too difficult and most players are not able to get by ghost #2 in the hallway. Maybe the hallway is too small, or the way the ghost patrols the hallway needs to be changed, but this indicates that players may get frustrated playing our game since it's too difficult. 

19. Next, let's create a graph that shows the sum of losses per player. Select the **+ ADD** button on the top left to add a new visualization.

20. Under Visual types, select a **Horizontal bar chart** visualization. 

21. Drag the **playerid** variable to be the value for the Y axis and the **losses** variable for the Value variable.

22. Your graph should look similar to this: 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/loss.png" /></p> 

* This visualizatoin shows the sum of losses per individual player. It shows that there are some players who are losing significantly more than other players. This graph may indicate certain players who are more likely to churn and stop playing your game because they are having a hard time winning. With this information, you can try to prevent player churn. for example, you can target these players with special items to help them get through the level to keep playing.

23. Let's create one more graph that shows our win/loss percentage. To do this, we need to add a new field that will be called WinLoss. Select the **+ ADD** button on the top left to add a **Add calculated field**.

24. Under **Function list** choose **avg**. For the **Calculated field name** put WinLoss. 

25. For **Formula** put `avg({wins} / {losses})`

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/calculated.png" /></p> 

26. Click **Create** to add your new calculated field.  

27. Now, let's visualize this. Select the **+ ADD** button on the top left to add a new visualization.

28. Leave the visual type as **AutoGraph** and simply select the new **WinLoss** variable. Click the drop down next to the **WinLoss** variable and show the number as a **Percent**.

29. Your graph should look similar to this: 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/WLP.png" /></p> 

Congratulations! You created batch visualizations in QuickSight. You can explore QuickSight more if you'd like to see what other visualizations you can make. 


<a id="TaskRealTime"></a>
[[Top](#Top)]
## Task 9: Setting up a near real-time pipeline with Amazon Kinesis Analytics, AWS Lambda, and Amazon CloudWatch

Now that the batch analytics portion is set up, let's move onto configuring the real-time portion of our pipeline so we can see data populating on a graph as it comes in. First, let's configure a Kinesis Analytics stream.

1. First, we need to start sending streaming data again. Go back to the **Kinesis Data Generator** and hit **Send data**. Keep data sending this time since we want to look at data as it comes in, do not stop the Kinesis Data Generator. 

2. Go to the AWS Management Console, select Services, and then choose **Kinesis**.

2. Under _Kinesis analytics applications_, choose **Create analytics application**.

3. Choose an **application name**, for example _serverless-analytics-stream_, and leave the **runtime** as SQL which should be selected by default.

4. Click **Create**.

5. We need to connect streaming data as a source. The source will be our Kinesis Data Firehose stream we configured in Task 3. We are sending data from the Kinesis Firehose stream directly to S3 to save our data for historical purposes and now we are also going to send it to a Kinesis Analytics application to run SQL queries on that data in real time. 

6. Click **Connect streaming data** and choose **Kinesis Firehose delivery stream** to add Kinesis Firehose as our streaming source. Choose the delivery stream you created in Task 3. 

7. Scroll down and click **Discover schema** to discover the schema of your data in your stream. It should look similar to below:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/schema.png" /></p> 

* If you are unable to discover the schema, it is probably because you are not sending data with the Kinesis Data Generator. 

8. Click **Save and continue**

9. Now, configure your real time analytics by clicking **Go to SQL editor**. If you are asked if you would like to start running your application, choose **Yes, start application**.

10. In the SQL editor, paste the following SQL query:

```
-- ** Continuous Filter ** 
CREATE OR REPLACE STREAM "data_stream" ("PlayerID" INTEGER, "Wins" INTEGER, "Losses" INTEGER, "COL_TimePlayed" INTEGER, "CaughtAt" INTEGER, "CaughtBy" INTEGER);
CREATE OR REPLACE PUMP "STREAM_PUMP" AS INSERT INTO "data_stream"
SELECT STREAM "PlayerID", "Wins", "Losses", "COL_TimePlayed", "CaughtAt", "CaughtBy"
FROM "SOURCE_SQL_STREAM_001"
WHERE "Losses" > 10;
```

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/stream.png" /></p> 

* This is a continous filter query that will filter the data in the stream. This query creates an output stream, which can be used to send to a destination. It creates a pump to insert data into the output stream, it selects all columns from the source stream that we want to filter through, and then it filters based on a _WHERE_ clause. In this case, the SQL query filters data where losses are greater than 10.

* For more information on how to create SQL queries for Kinesis Data Analytics, check out the SQL reference guide here:
https://docs.aws.amazon.com/kinesisanalytics/latest/sqlref/analytics-sql-reference.html

* You should be able to see results coming in real-time as shown below. If you look at tthe _Losses_ column, none of the losses are less than 10 due to the continuous filter SQL query. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/results.png" /></p> 

11. The last step for setting up the Kinesis Data Analytics stream is configuring a destination for the stream. The destination will be a Lambda function that will be configured next. 

12. Open a new AWS tab. It is time to configure a Lambda function that will consume data from the Kinesis Data Analytics stream and execute code to turn the data into a custom metric that will be published to a CloudWatch dashboard. In the AWS Management Console, select **Lambda**.

13. Click **Create function**.

14. Choose **Author from scratch**, which should be selected by default.

15. Give your function a name, for example _custom-cloudwatch-metrics_ and set the Runtime as **Python 3.8** 

16. Click **Create function** 

17. In this GitHub repository, you will see a file called _lambda_function.py_. Copy and paste the code in that file to the body of your Lambda function, which should look similar to below:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/lambda.png" /></p> 

* This code will take the filtered data sent from the Kinesis Data Analytics stream and send it to a CloudWatch dashboard as custom metrics. It does this using the CloudWatch Boto 3 SDK for Python. 

18. The last step to configure Lambda is making sure it has the appropriate permissions to publish metrics to CloudWatch. Scroll down to **Execution role** and click **View the custom-cloudwatch-metrics-role** as highlighted below. This will take you to the IAM management console where you can edit role permissions. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/lambdaiam2.png" /></p> 

19. In the IAM management console, select **Attach permissions**. Select the **CloudWatchFullAccess** policy and select **Attach policy**

20. Go back to your Lambda function and **Edit basic settings** to increase the timeout to 3 minutes.  

21. Finally, **Save** the Lambda function. 

22. Now go back to the open tab with your Kinesis Data Analytics stream to connect to a destination. Choose **Connect to a destination** and choose the destination to be an **AWS Lambda function**. 

23. Select the Lambda function you just created

24. For **In-application stream**, select **Choose an existing in-application stream** and choose data_stream, which is a stream that is created in the continous filter SQL query.

25. Leave the rest of the configurations as default and click **Save and continue**. It might take a couple minutes to save and connect your Lambda function as the destination to the stream. Your final Kinesis Data Analytics stream configurations should look like this:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/kinesisanalytics.png" /></p> 

* Check to make sure data is still being generated by the Kinesis Data Generator and is still being filtered with the Kinesis Data Analytics application.

26. Now it is time to configure a real-time CloudWatch dashboard! In the AWS Management Console, under Services choose **CloudWatch**. 

27. On the left-hand navigation pane, select **Dashboards** and click **Create dashboard**. 

28. Give your dashboard a name, for example _serverless-analytics_ and click **Create dashboard**.

29. Add a **Stacked area** widget and click **Configure**. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/widget.png" /></p> 

30. You should see a **Custom Namespace** called **serverless-analytics-demo**. Click that custom namespace, and you should see **6 Metrics with no dimensions**. Click into that and you should see familiar metrics, like _caughtAt_, _caughtBy_, and more. 

31. Select both the **wins** and **losses** metrics to be plotted on this graph. At the top right, select the time interval to be a **Custom 1 minute** interval and on the refresh button drop down choose **Auto refresh every 10 Seconds**. 

Your configurations should look like the following:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/widgetconfig.png" /></p> 

32. You can also edit the widget to change the time period of each data point for Wins and Losses to be 1 second if you want. 

33. Save your dashboard. 

34. Turn on live data by clicking **Actions**, then **Live data**, and turn it **On** like below:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/livedata.png" /></p> 

Finally, you should be able to see a graph similar to below that populates data in real time!

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/dashboard.png" /></p> 

* This graph shows live data coming in of your players wins and losses while they play your game! Try seeing what other dashboard widgets you can create. You can come up with a live dashboard similar to the one below:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/gdc.png" /></p> 

* This is a sample dashboard that shows live data for wins and losses over time, the amount of time played, and p90 wins and losses. 

Congratulations! You created real-time visualizations in CloudWatch using Kinesis Data Analytics and Lambda. 

<a id="cleanup"></a>
[[Top](#Top)]

## Clean up 

Now that you have successfully created a batch and near real-time serverless analytics pipeline and integrated it with a Unity game, you can clean up your environment by spinning down AWS resources. This helps to ensure you are not charged for any resources that you may accidentally leave running.

* Go to the AWS Management Console, select Services, and click **S3**. Find the S3 bucket that is your data lake, click on it, and you should see a **Delete** button.

* Go to the AWS Management Console, select Services, and click **Kinesis**. Find the Kinesis Firehose Delivery Stream you created and click it to view details about it. On the top right, you should see a button to **Delete delivery stream**. Do this for your Kinesis Data Analytics stream too. 

* Go to the AWS Management Console, select Services, and click **CloudFormation**. Find the stack you spun up to configure the Kinesis Data Generator, click it and there is a **Delete** button at the top. This will delete the CloudFormation stack and any resources it has spun up.

* In the AWS Management Console, find the **Glue** dashboard. Go to **Databases** on left-hand side and find the Glue Data Catalog you created. Drop down the **Action** button and hit **Delete database**. 

* Still working in the Glue dashboard, on the left-hand side select **Crawlers**. Select your crawler, find the drop down **Action** button, and hit **Delete crawler**.

* When you run queries in Athena, query results are saved in an S3 bucket in your account for each region you are working in. For example, if this lab was done in US-WEST-2, there should now be an S3 bucket in your account titled something similar to: _aws-athena-query-results-us-west-2-YOUR-ACCOUNT-ID_. You can delete this bucket as well so you are not paying to store the results of the query you ran for this lab. 

* In the AWS Management Console, go to **QuickSight** dashboard. On your analysis that you created, click the three little dots to the right and delete your analysis. 

* Go to your **CloudWatch** dashboard. On the widget you created, click settings and choose **Delete**. 

* Go to the **Lambda** management console. Click the Lambda functions you created. Click the **Actions** drop down and then **Delete** the functions. 

* Go to the **API Gateway** management console. Click the API Gateway you created. Click the **Actions** drop down and then **Delete** the API Gateway. 

<a id="additionalreading"></a>
[[Top](#Top)]

## Appendix - additional reading

Building Big Data Storage Solutions (Data Lakes) for Maximum Flexibility
https://docs.aws.amazon.com/whitepapers/latest/building-data-lakes/building-data-lake-aws.html

AWS SDK for .NET Developer Guide  
https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/welcome.html

Amazon Game Tech Blog   
https://aws.amazon.com/blogs/gametech/

How to integrate the AWS .NET SDK for games using C#  
https://aws.amazon.com/blogs/gametech/how-to-integrate-the-aws-net-sdk-for-games-using-csharp/

Data Lakes and Analytics on AWS  
https://aws.amazon.com/big-data/datalakes-and-analytics/

Gaming Analytics Pipeline Solution  
https://aws.amazon.com/solutions/gaming-analytics-pipeline/

AWS Big Data Blog  
https://aws.amazon.com/blogs/big-data/category/analytics/




