![AWS_logo_RGB.png](http://d2a4jpfnohww2y.cloudfront.net/cognito/AWS_logo_RGB.png)

<a id="Top"></a>
## Serverless Data Analytics Lab
This lab will guide you through creating a serverless analytics pipeline and integrating it into a Unity game using the AWS SDK for .NET. This lab will focus on using serverless services like Amazon S3, Amazon Kinesis, AWS Glue, Amazon Athena, and Amazon QuickSight. In this lab, you will build out the following architecture:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/analytics2.png" /></p> 

## Agenda 

* [Overview](#Overview)
* [Task 1: Setting up Prerequisites and Permissions](#Task1)
* [Task 2: Creating an Amazon S3 Data Lake and Kinesis Firrehose Stream for Data Ingestion](#Task2)
* [Task 3: Integrating AWS with Unity](#Task3)
* [Task 4: Populating Data Lake with Amazon Kinesis Data Generator](#Task4)
* [Task 5: Using AWS Glue to Discover Data](#Task5)
* [Task 6: Querying Data with Amazon Athena](#Task6)
* [Task 7: Discovering Insights with Amazon QuickSight](#Task7)
* [Clean Up](#cleanup)
* [Appendix - Additional Reading](#additionalreading)


<a id="Overview"></a>
[[Top](#Top)]

## Overview

### Why use analytics in my game?

Using analytics is very important to improve your game and keep players around. Incorporating an analytics pipeline to your game can help you create more engaging games by doing data driven game development. You can learn how to optimize the game play experience so you can attract more players and increase player engagement. It can help with anomaly detection by identifying abusers, cheaters, and player churn. It can also help you improve your game infrastructure by better understanding peak usage times so you know when to scale. Finally, data analytics can help with revenue generation by encouraging purchases, targeted ads, and content recommendations. 


### Serverless Analytics Pipeline on AWS

Serverless applications don’t require you to provision, scale, or manage any servers. You can build them for nearly any type of application or backend service, and everything required to run and scale your application with high availability is handled for you. A lot of companies are making the move towards serverless, even for their analytics workloads. AWS has an expansive ecosystem of services that can be used to build out a robust anlaytics pipeline for your game so that you can discover useful insights on player and game data. Some services that you can use for batch analytics include:

##### Amazon S3

Amazon S3 provides durable object storage in the AWS cloud. It makes for a great data lake solution because of its virtually unlimited scalability. As your data grows, you can increase storage from gigabytes to petabytes of data. It allows you to decouple storage and compute so that you can scale both independently as needed. This can act as a central data store for all of your game data. 

##### Amazon Kinesis 

Amazon Kinesis makes it easy to collect, process, and analyze real-time streaming data so you can get timely insights and react quickly to new information. You can use Kinesis for real-time delivery of in-game data collected from game servers and clients to be stored in your S3 data lake. 

##### AWS Glue

AWS Glue is a fully managed extract, transform, and load (ETL) service that makes it easy to prepare and load data for analytics. You can use AWS Glue to discover the data in your S3 data lake to make it searchable, queryable, and available for ETL. 

##### Amazon Athena

Once your data has been discovered using AWS Glue, you can use Amazon Athena to query your data using standard SQL. With Athena, you can query exactly the data you want to analyze. 

##### Amazon QuickSight

Once you query the data you are interested in analyzing, you can use Amazon Quicksight as a business intelligence service to discover insights about your game data. You can create and publish interactive dashboards and visualizations. You can even discover hidden trends and do forecasting using machine learning. With Quicksight, you can answer questions about your game - is it too hard? Is it too easy? Are your players engaged and going to stick around? 

### Unity

Unity is a cross-platform game engine developed by Unity Technologies that is used to create the core game that will be the basis of this lab. Unity uses a C++ runtime and C# for scripting, so to be able to add AWS functionality to the game the AWS SDK for .NET is included as a package in Unity.

### Getting Started

This lab will focus specifically on building out a serverless analytics pipeline and integrating it into a Unity game. You will first create an S3 data lake and integrate that into a Unity game using the AWS SDK .NET. You will populate the data lake with Kinesis, discover the data using Glue, practice querying the data with Athena, and then create visualizations from the data with QuickSight. 
 
 <a id="Task1"></a>
[[Top](#Top)]

## Task 1: Setting up Prerequisites and Permissions
 
 ### Prerequisites

For the purposes of this lab, you will be using the AWS Management Console as well as Unity. You will need to have:

* An **AWS account** with an appropriate level of permissions to use the services needed for this lab (S3, Kinesis, Glue, Athena, QuickSight). Follow the link to create and activate a new AWS account if you do not have one already: https://aws.amazon.com/premiumsupport/knowledge-center/create-and-activate-aws-account/
* **Unity 2019.1.0** - Download Unity and Unity Hub from this link: https://unity3d.com/get-unity/download/archive
* AWS Command Line Interface (CLI) - https://aws.amazon.com/cli/

This lab works for both Mac and Windows. If you already have these prerequisites installed and credentials configured, you can skip to [[Task2](#Task2)]


### Setting up Permissions

First, you will need to create an IAM user with the appropriate permissions needed to do the lab if you do not have one already. AWS Identity and Access Management (IAM) enables you to manage access to AWS services and resources securely. Using IAM, you can create and manage AWS users and groups, and use permissions to allow and deny their access to AWS resources. It is highly recommended that you do not use the default root user of your AWS account and instead provision your own IAM user for security purposes.

1. Sign into your AWS account and go to the IAM landing page by clicking **Services > IAM**.

2. Click **Users** on the left-hand navigation pane and then select **Add user**.

3. Give your user a user name and make sure to enable **Programmatic access** so that you can download an access key and secret access key. Also enable **AWS Management Console access** so that you can give your user the ability to sign-in to the AWS Management console.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/IAM1.png" /></p> 

4. Select **Next: Permissions** and choose **Attach existing policies directly**. Choose the AdministratorAccess policy to add to your user.

**SECURITY DISCLAIMER:** Here, you are adding full administrator access for simplicity of lab purposes. However, it is best practices that with IAM you assign fine-grained permissions to AWS services and to your resources. If you want to make your permissions more fine-grained and not use admin permissions, you can add permissions for only the services that will be used in this lab, including S3, Kinesis, Glue, Athena, and QuickSight. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/IAM2.png" /></p> 

5. Select **Next: Tags**, **Next: Review** and finally **Create user**.

6. Make sure you **Download .csv** to download and save your access key ID and secret access key for use later.

7. Sign into the AWS Management Console with the IAM credentials you just created. 

### Installing the AWS CLI

Now that you have created a user, you can install the AWS CLI. You are doing this to configure AWS credentials easily to your local machine to be able to complete the lab. 

1. Installing the AWS CLI - https://aws.amazon.com/cli/

2. Configure AWS credentials by opening a terminal and running:

`aws configure`

* It will prompt you for your access key ID, your secret access key ID, a region name, and an output format.
* Enter the access key and secret access key that you downloaded in the last step.
* Choose a region name. You can choose whatever region you want to work in, as long as that region supports all the services that this lab needs. This lab uses us-west-2 by default, so choose that if you'd like. 
* For the output format, just leave that default by pressing enter on your keyboard.

You are done setting up the prerequisites needed for this lab.

 
<a id="Task2"></a>
[[Top](#Top)]

## Task 2: Creating an Amazon S3 Data Lake and Kinesis Firehose Stream for Data Ingestion 

The first step is to set up your data storage and your ingestion mechanism. For your data storage, you are going to use Amazon S3, which will act as your centralized data lake for all your game data. 

1. Sign into the **AWS Management Console** and on the Services menu, click **S3**. 
2. Click **+ Create bucket**.
3. Enter a bucket name. It has to be globally unique across all existing buckets in S3. This lab will use a bucket named _serverless-games_.
4. Choose the region for this bucket. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/1.png" /></p> 

5. Click **Create**.

You have created your S3 data lake! Now you need to set up an ingestion mechanism so you can stream data from your game to your S3 data lake. You can do this using an Amazon Kinesis Data Firehose stream. Kinesis Firehose is scalable so it allows you to ingest records from many clients simultaneously. It can stream data to multiple destinations besides just S3 and it integrates easily with other AWS services like Kinesis Data Analytics to process your streaming data using standard SQL. 

6. In the AWS Management Console, go to Services and click **Kinesis**.

7. In the top right corner, you will see a section for Kinesis Firehose delivery streams. Click **Create delivery stream**.

8. Give your stream a name. This lab will use the name _serverless-games-stream_. 

9. Under Choose source, keep it the default as _Direct PUT or other sources_.

10. Hit **Next**.

11. Leave these configurations as default and hit **Next** again.

12. Under Select destination, make sure **Amazon S3** is selected.

13. On the same page under S3 destination, choose the S3 bucket you created previously. 

14. You can take a look at all the other configuration options and explore them if you want but for now leave them all default and hit **Next**.

15. Scroll down to IAM role. This is the Identity and Access Manamgenent role that you need to specify to give Kinesis the appropriate permissions it needs to access your S3 bucket and any other resources it may need. Click **Create new or choose**.

16. This will open up a new tab like the one below where you can create a new IAM role. Select **Allow**.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/10.png" /></p> 

17. You should be redirected back to the tab where you are configuring your Kinesis Firehose stream. Review your configuration settings and finally select **Create delivery stream**. You should now see your newly created stream on the Kinesis Firehose dashboard. It will take a minute or two to create, but once it says the status is active you can click into it to find details. 

<a id="Task3"></a>
[[Top](#Top)]

## Task 3: Integrating AWS with Unity

Now that you have your data storage and ingestion mechanism, it is time to create a sample project in Unity that you will begin integrating your analytics pipeline with. 

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
   
9.	Browse to the **Plugins** folder in Assets. In this example, you will use the **AWS SDK for .NET** to be able to use AWS services in your game. Here, you can see the different plugins that have been included in this Unity project that are necessary to be able to send game data to Kinesis to store it in S3.  

   * **Note:** There are other ways that you can incorporate the use of AWS into your game depending on your use case. The AWS SDK for .NET is a valid option for doing so. It is recommended that you use this SDK instead of using the AWS Mobile SDKs for iOS, Android, and Unity because these are currently outdated. Instead, use the main AWS SDK for the language that you are programming your game in. Since Unity uses C#, you will use the AWS SDK for .NET which supports C#.

     * Also, when creating your own Unity game, you must make sure to follow these steps to change settings to be able to add .NET SDK assemblies to Unity as plugins.

       * Navigate to Edit -> Project Settings -> Player -> Other Settings
       * Change Scripting Runtime Version to .NET 4.x Equivalent
       * Change the Scripting Backend to Mono
       * Change the API Compatibility Level to .NET Standard 2.0

     * These steps are not necessary now to be able to do this lab since the Unity game has been provided for you, but are necessary when developing your own game that uses the .NET SDK. 

Now that you have your Unity sample game open and you have explored around a bit, it is time to begin coding some AWS functionality into the game. You will add code that will be able to send data to your Kinesis Data Firehose stream so that you can store all data in your S3 bucket. 

10. Navigate to the **Scripts** folder in Assets and open up the **KinesisFirehose.cs** script to be edited in Visual Studio or whatever editor you prefer. 

* This is the script that has been created to send game data to your Kinesis Firehose stream. You will need to write some code to make your script function correctly. Let’s walk through this together.

* The first part of this script (lines 19-28) references different namespaces that are needed to help create the functionality that you want to include in your game. This references the plugins from the AWS SDK for .NET that you looked at earlier in the Plugins directory in the Assets folder.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/kinesis1.png" /></p> 

* For example, you can see **using Amazon.KinesisFirehose** (line 25) which allows you to use the Amazon Kinesis API. This will allow you to do things like upload a record to a Kinesis stream. 

* Next, you need to declare variables that are necessary to be used in the script. Most of the variables are already defined for you.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/kinesis2.png" /></p> 

* The first variable defines the Amazon Kinesis Firehose Client. It is the client that you need to include and intialize so that you can access the Kinesis Firehose service to make API calls. 

11. On line 37, define the **Region** you created your Amazon Kinesis Firehose stream in. This lab is done in US West 2 for example. If you used a different region, make sure to change it here. To find the region code for the region you are using, click here: https://docs.aws.amazon.com/general/latest/gr/rande.html

12. On line 39, define the **name** of your Kinesis Firehose stream. This lab uses _serverless-games-stream_. If your stream has the same name, keep this line as is. If you used a differrent name, make sure to update it here. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/kinesis3.png" /></p> 

* Lets take a closer look at the other variables that are defined. On line 41, a representation of the game ending script is defined as _gameEnding_ - this script determines if the player wins or loses and resets the game. This is necessary because you want to collect this data to be analyzed and also send logs every time the game resets for the purpose of this lab.

* On line 43, a boolean variable _sent_ is declared to know if the records have been sent to Kinesis once or not to avoid sending multiple copies. This variable essentially checks to make sure the asychronous function you will create to send data to Kinesis is finished running before firing it off again.

* On line 44, a _recordData_ variable is declared that will contain the information to be sent to Kinesis.

* Lines 46-49 define sample data that is collected to be analyzed. The _playerid_ is hardcoded to be 1 for the purpose of this lab. The _timeplayed_ is the time the player has spent playing the game. Then, _losses_ is the amount of times the player has lost while _wins_ is the amount of time the player has won.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/kinesis4.png" /></p>

* The _start_ method runs the game on start of the scene. Here, the Kinesis _client_ is initialized, _sent_ is set to false, and _recordData_ is initialized as a new hash table.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/kinesis5.png" /></p>

Lets take a look at the _update_ method now. This method checks to see if the player loses or wins. If the player has lost, increment the _losses_ variable by 1. If the player has won, increment the _wins_ variable by 1. The _sent_ variable is set to true since you are firing off an asynchronous method to send data to Kinesis and want to wait for that task to finish before triggering it again. Finally, on line 71 the _WriteRecord()_ method is called. Time to begin writing code!

13. Look for the _WriteRecord()_ method. Right now, it consists of a try-catch block, where you will try to send a record to Kinesis and catch any exceptions that may occur.

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/kinesis6.png" /></p>

* At the beginning of this method, we have some variables that we define. On line 80, _timeplayed_ is set to be the current time. On lines 82-85, we add the current value of _playerid, timeplayed, losses_, and _wins_ to the _recordData_ hash table/ on line 88, _recordData_ is converted to a byte array because Kinesis Firehose expects a memory stream when you provide it records. 

14. Find where it says to _//Fill in code here!_ on line 91. First, create a **PutRecordRequest** which contains the Kinesis Firehose stream name and the data you want to send to the stream. You can put this code right after the comments, creating a new line on 93. If you need help with writing the code, the final KinesisFirehose.cs script with all the code is in this GitHub repository for you to download and use as a reference. 

        //Create a PutRecordRequest
        PutRecordRequest putRecordRequest = new PutRecordRequest
        {
           DeliveryStreamName = streamName,
           Record = new Record
           {
               Data = new MemoryStream(dataAsBytes)
            }
         };
	  
	  
* Here, you are defining the delivery stream name to be the name of your Kinesis stream. You are also defining the record you want to send, which is a memory stream of the player data.

15. Then, you want to put that record into your Kinesis stream using a **PutRecordAsync** request. Make sure this goes after the PutRecordRequest code from above.

 	      // Put record into the DeliveryStream
              PutRecordResponse response = await client.PutRecordAsync(putRecordRequest);


* This will actually send your data blob to Kinesis and return a response if its successful or not. 

16. **Save** your file - you are done! 

17. **MainScene** should already be open, but if it's not, go to Assets > Scenes > and open MainScene. It is time to begin playing the game to test it out.

18. Hit **play**. The goal is to avoid enemies and escape from the haunted house. Play around a bit - lose a couple times and try to win if you want. This will send some sample data to your Kinesis stream which will end up in your S3 bucket. Don't worry about trying to send a lot of data now, you will ingest a lot of sample data to your bucket in the next step. 

19. Stop playing the game and monitor how your Kinesis Firehose stream is performing. Go to the **AWS Management Console**, click **Kinesis** and find your **Kinesis Firehose delivery stream**. 

20. Click into your stream to see details about it and select the **Monitoring** tab. You can see Amazon CloudWatch metrics, similar to the ones shown below. These metrics show the amount of incoming records, the amount of records successfully delivered to S3, and more. Data might not be immediately visible on these graph due to the buffer interval of your stream. If you do not see data immediately, wait a few minutes and refresh. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/kinesis7.png" /></p>

28. In the **AWS Management Console**, go to **S3** and find the S3 bucket you created earlier in this lab. Look at the contents of this S3 bucket. You should see data in there that looks similar to this:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/kinesis8.png" /></p>

27. **Download** one of the files by clicking on it and hitting download. Take a look at the contents. You should see your game data in JSON format like below:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/kinesis9.png" /></p>

Congratulations! You successfully created an S3 data lake, a Kinesis Firehose stream, and integrated it with a Unity game using the AWS SDK .NET.

<a id="Task4"></a>
[[Top](#Top)]

## Task 4: Populating Data Lake with Amazon Kinesis Data Generator

Now you are able to successfully send your own player data to S3 as you play the game. Right now, this is a small amount of data since you are only one player. You want to simulate this on a larger scale with more data so you can see useful visualizatitons. For the purpose of this lab, you will use the Kinesis Data Generator to simulate data. 

1. The **Amazon Kinesis Data Generator** is found at this link: https://awslabs.github.io/amazon-kinesis-data-generator/web/producer.html. Open this link - you should see a webpage similar to the one below. 

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/11.png" /></p> 

14. Click the **Help** button. This page will walk you through how to configure Kinesis Data Generator with your AWS account. Follow the steps to **Create a Cognito User with CloudFormation**.

15. This will bring you to the AWS Management Console CloudFormation dashboard. You should see the followng:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/12.png" /></p> 

16. Hit **Next**. Create a username and password that will be used to sign into the Kinesis Data Generator. Hit **Next** again.

17. Leave all other configuration default and hit **Next** twice more. 

18. Review your configurations and check the box at the bottom that acknowledges that this CloudFormation template might create IAM sources. Finally click **Create stack**. It will take a few minutes for the CloudFormation stack to finish creating.

19. When it is finished, on the _Outputs_ page of the stack you will see a **KinesisDataGeneratorUrl**. Open this link. You will use this link to sign in to a page that looked the same before. Sign in using the username and password you just configured in the CloudFormation template. 

20. When the log-in is successful, you will see some fields that you can start configuring to send sample data to populate your S3 data lake. 

      * Set the **Region** as the same one you created your Kinesis Firehose stream in.
      * Set the **Stream** as the one you created in Task 2.
      * On the **Record template** under Schema Discovery Payload, put the following:
 
```
{
"Time Played":{{random.number(10000)}},
"Wins":{{random.number(50)}},
"PlayerID":{{random.number(50)}},
"Losses":{{random.number(50)}}
}
```

This data represents random players playing your game. Your final configurations should look similar to this:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/13.png" /></p> 

21. Hit **Send data**. You should see data starting to send. Let the data generator send a couple thousand records (3000 records for example would be a good stopping point) and then finally hit **Stop sending data to Kinesis**. 

22. Go back to the AWS Management Console tab with your Kinesis Firehose delivery stream open. Click into the stream to view the stream details if you are not viewing them already.

23. Select the **Monitoring** tab to view metrics like you did in Task 2. You should see something similar to what is shown below to verify the Kinesis Data Generator is working:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/14.png" /></p> 

24. Go back to your S3 bucket to look at the contents and **verify** the data has been delivered. 

Now you have a lot of sample player data to work with for the rest of the lab. 


<a id="Task5"></a>
[[Top](#Top)]

## Task 5: Using AWS Glue to Discover Data

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

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/19.png" /></p> 

Congratulations! You have successfully used AWS Glue to create a crawler and populate a Glue Data Catalog to discover the data in your S3 data lake.


<a id="Task6"></a>
[[Top](#Top)]

## Task 6: Querying Data with Amazon Athena

Now that you have made your data discoverable, you can query your data for exactly what you want to analyze. 

1. Go to AWS Management Console, click Services, then select **Athena**.

2. On the left hand side under Database, select the **Glue Data Catalog** you just created. In the case of the lab, it is the _serverless-catalog_.

3. Create a new **query** in the middle pane of the Athena Dashboard. Here you can write standard SQL to run queries against your Glue Data Catalog. Run the following query:

```
SELECT * FROM "serverless-catalog"."23" WHERE "serverless-catalog"."23"."time played" > 3600
```

* You might need to change where it says _serverless-catalog_ to the name of your own Glue Data Catalog. You also might need to change the table name to match the name of yours, which you can find in the left-hand Database pane. For reference, the table name here is "23" so change this to match your table name. Here we are querying all the data where the players had spent more than 3600 seconds, or 1 hour, playing the game. 

* Your final result should look like this:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/20.png" /></p> 

4. After you run the query and the results display on the dashboard, click **Create** and then **Create view from query**. Give it a name. This lab uses _data-view_ as the name. Click **Create**. 

Congratulations! You have finished querying your data using Amazon Athena. 

<a id="Task7"></a>
[[Top](#Top)]

## Task 7: Discovering Insights with Amazon QuickSight

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

11. Once your data has been imported, you can start messing around with QuickSight to see what visualizations you can create. Select the **+ ADD** button on the top left to add a visualization. 

12. Under Visual types, select a **Line chart** visualization. 

13. Drag the **time played** variable to be the value for the X axis.

14. Drag the wins and losses variables to be the green values. Click the drop down arrow next to both of these variables to sort them 

15. After adding all three variables, click the drop down arrow next to the time played variable to sort it to be **ascending** by time played. You should end up with something that looks similar to this:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/23.png" /></p> 

* This graph shows the average losses and average wins over time. In this case, the data is randomized, but in practice a graph like this could indicate that if your players are winning too much, the game might be too easy and they might get bored and stop playing. On the other hand, it could indicate that if your players are losing too often, the game might be too hard, and as a result the players could get frustrated and stop playing. 

16. You can also add filters to you graph. On the left-hand panel, select **Filter** and create one for **playerid**. Click on **playerid** to expand it.

17. Hit **Apply**. Your final result should look something like this:

<p align="center"><img src="http://d2a4jpfnohww2y.cloudfront.net/serverless-analytics/24.png" /></p> 

* Instead of showing all the win/loss data for all your players, adding a filter can help you take a look at the data for a subset of players or one individual player. 

Congratulations! You created a sample visualization in QuickSight to start analyzing your data. You can explore QuickSight more if you'd like to see what other insightful visualizations you can make. 


<a id="cleanup"></a>
[[Top](#Top)]

## Clean Up 

Now that you have successfully created a serverless analytics pipeline and integrated it with a Unity game, you can clean up your environment by spinning down AWS resources. This helps to ensure you are not charged for any resources that you may accidentally leave running.

* Go to the AWS Management Console, select Services, and click **S3**. Find the S3 bucket that is your data lake, click on it, and you should see a **Delete** button.

* Go to the AWS Management Console, select Services, and click **Kinesis**. Find the Kinesis Firehose Delivery Stream you created and click it to view details about it. On the top right, you should see a button to **Delete delivery stream**.

* Go to the AWS Management Console, select Services, and click **CloudFormation**. Find the stack you spun up to configure the Kinesis Data Generator, click it and there is a **Delete** button at the top. This will delete the CloudFormation stack and any resources it has spun up.

* In the AWS Management Console, find the **Glue** dashboard. Go to **Databases** on left-hand side and find the Glue Data Catalog you created. Drop down the **Action** button and hit **Delete database**. 

* Still working in the Glue dashboard, on the left-hand side select **Crawlers**. Select your crawler, find the drop down **Action** button, and hit **Delete crawler**.

* When you run queries in Athena, query results are saved in an S3 bucket in your account for each region you are working in. For example, if this lab was done in US-WEST-2, there should now be an S3 bucket in your account titled something similar to: _aws-athena-query-results-us-west-2-YOUR-ACCOUNT-ID_. You can delete this bucket as well so you are not paying to store the results of the query you ran for this lab. 

* In the AWS Management Console, go to **QuickSight** dashboard. On your analysis that you created, click the three little dots to the right and delete your analysis. 


<a id="additionalreading"></a>
[[Top](#Top)]

## Appendix - Additional Reading

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




