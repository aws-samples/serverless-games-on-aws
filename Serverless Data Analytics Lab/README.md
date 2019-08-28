![AWS_logo_RGB.png](img/AWS_logo_RGB.png)

<a id="Top"></a>
## Serverless Data Analytics Lab
This lab will guide you through creating a serverless analytics pipeline and integrating it into a Unity game using the AWS SDK for .NET. This lab will focus on using serverless services like Amazon S3, Amazon Kinesis, AWS Glue, Amazon Athena, and Amazon QuickSight. In this lab, you will build out the following architecture:

<p align="center"><img src="img/architecture.png" /></p> 

## Agenda 

* [Overview](#Overview)
* [Task 1: Creating an Amazon S3 Data Lake and Ingesting Player Data from Unity](#Task1)
* [Task 2: Populating Data Lake with Amazon Kinesis Data Firehose](#Task2)
* [Task 3: Using AWS Glue to Discover Data](#Task3)
* [Task 4: Querying Data with Amazon Athena](#Task4)
* [Task 5: Discovering Insights with Amazon QuickSight](#Task5)
* [Clean Up](#cleanup)
* [Appendix - Additional Reading](#additionalreading)



<a id="Overview"></a>
[[Top](#Top)]

## Overview

### Serverless Analytics Pipeline on AWS

Serverless applications don’t require you to provision, scale, or manage any servers. You can build them for nearly any type of application or backend service, and everything required to run and scale your application with high availability is handled for you. A lot of companies are making the move towards serverless, even for their analytics workloads. AWS has an expansive ecosystem of services that can be used to build out a robust anlaytics pipeline for your game so that you can discover useful insights on player and game data. These services include:

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

### Prerequisites

For the purposes of this lab, you will be using the AWS Management Console as well as Unity. You will need to have:

*	An AWS account with an appropriate level of permissions to use the services needed for this lab (S3, Kinesis, Glue, Athena, QuickSight)
*	Unity 2019.1.0
*	Visual Studio 2019


<a id="Task1"></a>
[[Top](#Top)]

## Task 1: Creating an Amazon S3 Data Lake and Ingesting Player Data from Unity

The first step is to create an Amazon S3 bucket, which will act as your centralized data lake for all your game data. 


1.	Sign into the AWS Management Console and on the Services menu, click **S3**. 
2. Click **+ Create bucket**.
3. Enter a bucket name. It has to be globally unique across all existing buckets in S3. This lab will use a bucket named _serverless-games_.
4. Choose the region for this bucket. Take note of both the bucket name and region for later.

<p align="center"><img src="img/1.png" /></p> 

5. Click **Create**.

You have created your S3 data lake! Now it is time to integrate it into a sample Unity game. You can open and explore a sample project in Unity that you will use to send game data to your S3 bucket. 

6.	**Download or clone** the files in this repository to the directory of your choice.
7. Open the project in Unity. This can be done by opening up **Unity Hub**, selecting **Add**, and then navigating to the project folder _“Serverless-Games-on-AWS”_ containing the files you just downloaded. Inside this, select the folder _“Serverless Data Analytics Lab”_ containing the Unity project files. It should look similar to this:

<p align="center"><img src="img/2.png" /></p> 

7.	Once you have opened the project, browse the assets that make up this game.

     * The core game has been built already for you in Unity. You will take this core game and add functionality to it using AWS as you progress through this lab. The baseline of this game has been built using the free John Lemon’s Haunted Jaunt assets from the Unity Asset store, which can be found here for your reference: https://learn.unity.com/project/john-lemon-s-haunted-jaunt-3d-beginner

     * The game has already been built for you, but if you want to familiarize yourself with Unity you can run through the tutorial to build the game yourself. Otherwise, browse the different assets folders – take a look at the Scripts, Prefabs, Animation folders and more to see what gives the game its functionality. 

<p align="center"><img width="500" src="img/3.png" /></p> 

8.	Browse to the **Plugins** folder in Assets. In this example, you will use the **AWS SDK for .NET** to be able to use AWS services in your game. Here, you can see the different plugins that have been included in this Unity project that are necessary to be able to send game data to S3.  

   * **Note:** There are other ways that you can incorporate the use of AWS into your game depending on your use case. The AWS SDK for .NET is a valid option for doing so. It is recommended that you use this SDK instead of using the AWS Mobile SDKs for iOS, Android, and Unity because these are currently outdated. Instead, use the main AWS SDK for the language that you are programming your game in. Since Unity uses C#, you will use the AWS SDK for .NET which supports C#.

     * Also, when creating your own Unity game, you must make sure to follow these steps to change settings to be able to add .NET SDK assemblies to Unity as plugins.

       * Navigate to Edit -> Project Settings -> Player -> Other Settings
       * Change Scripting Runtime Version to .NET 4.x Equivalent
       * Change the Scripting Backend to Mono
       * Change the API Compatibility Level to .NET Standard 2.0

     * These steps are not necessary now to be able to do this lab since the Unity game has been provided for you, but are necessary when developing your own game that uses the .NET SDK. 

Now that you have your Unity sample game open and you have explored around a bit, it is time to begin coding some AWS functionality into the game. You will add code that will be able to send data to your S3 bucket. 

9. Navigate to the **Scripts** folder in Assets and open up the **S3** script to be edited in Visual Studio or whatever editor you prefer. 

* This is the script that has been created to send game data to S3. You will need to write some code to make your script function correctly. Let’s walk through this together.

* The first part of this script (lines 1-8) references different namespaces that are needed to help create the functionality that you want to include in your game. This references the plugins from the AWS SDK for .NET that you looked at earlier in the Plugins directory in the Assets folder. 

<p align="center"><img src="img/4.png" /></p> 

* For example, you can see **using Amazon.S3** (line 6) which allows you to use the Amazon S3 API. This will allow you to do things like upload an object to a bucket or get an object from a bucket, for example. 

* Next, you need to declare variables that are necessary to be used in the script. Most of the variables are already defined for you.  

<p align="center"><img src="img/5.png" /></p> 

* The first two variables define the key name and Amazon S3 Client. The key name is the object key which uniquely identifies the object in a bucket. The IAmazonS3 client is the interface that needs to be declared to access S3. This interface has many methods that can be used to do things in S3 - like uploading objects to a bucket, listing objects in a bucket, deleting a bucket, and more. 

10. On line 17, define the **Region** you created your S3 bucket in. This lab has been done in US West 2 for reference. 

11. On line 19, add your **S3 bucket name** between the quotation marks. Your variables should look similar to this:

<p align="center"><img src="img/6.png" /></p> 

* Lets take a closer look at the other variables that are defined. On line 21, a representation of the game ending script is defined as _gameEnding_ - this script determines if the player wins or loses and resets the game. This is necessary because you want to collect this data to be analyzed and also send logs every time the game resets for the purpose of this lab. 

* On line 23, a boolean variable _sent_ is declared to know if the logs have been sent to S3 once or not to avoid sending multiple copies. This variable essentially checks to make sure the asychronous function you will create to send data to S3 is finished running before firing it off again. 

* On line 24, a _data_ variable is declared that will contain the information to be sent to S3.

* Lines 26-29 define sample data that is collected to be analyzed. The _playerid_ is hardcoded to be 1 for the purpose of this lab. The _timeplayed_ is the time the player has spent playing the game. Then, _losses_ is the amount of times the player has lost while _wins_ is the amount of time the player has won. 

<p align="center"><img src="img/7.png" /></p> 

* The start method runs the game on start of the scene. Here, the S3 client is initialized, _sent_ is set to false, and _data_ is initialized as a new hash table.

* Lets take a look at the update method now. This method checks to see if the player loses or wins. If the player has lost, increment the _losses_ variable by 1. If the player has won, increment the _wins_ variable by 1. The _sent_ variable is set to true since you are firing off an asynchronous method to sent data to S3 and want to wait for that task to finish before triggering it again. Finally, on line 48 the _WritingAnObjectAsync()_ method is called. Time to begin writing code!

12. Look for the _WritingAnObjectAsync()_ method. Right now, it consists of a try-catch block, where you will try to send an object to S3 and catch any exceptions that may occur. Find the comment where it says to _Fill in code here_ which is in the try block. First, define the **keyName** which is the name of the object to be uploaded. You will just set this as the current date and time.

```
keyName = DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
```    
13. Define **timeplayed** to be the current time.

```
timeplayed = Time.time;
```

14. Add the current value of _playerid_, _timeplayed_, _losses_, and _wins_ to the **data** hash table. 

```
data.Add("PlayerID", playerid);
data.Add("Time Played", timeplayed);
data.Add("Losses", losses);
data.Add("Wins", wins);
```

15. Next, you need to create a **PutObjectRequest** to send a new object to your S3 bucket.

```
var putRequest1 = new PutObjectRequest
{
  BucketName = bucketName,
  Key = keyName,
  ContentBody = JsonConvert.SerializeObject(data),
  ContentType = "application/json"
};
```

* Here, you are creating a new _PutObjectRequest_. You need to specify the bucket name of your S3 bucket that you want to upload data to. You specify the key name and the content of the object. The content of the object is the _data_ hash table variable converted to JSON format. The content type of the object is defined to be JSON. 

16. Finally, you need to actually call the **PutObjectAsync()** method on the S3 client. This is an asynchronous operation that adds the object to your S3 bucket.

```
PutObjectResponse response1 = await client.PutObjectAsync(putRequest1);
```

17. **Save** your file - you are done! 

18. In Assets, find the Scenes folder. Look for **SampleScene** a nd open it. It is time to begin playing the game to test it out.

19. Hit **play**. The goal is to avoid enemies and escape from the haunted house. Play around a bit - lose a couple times and try to win if you want. This will send some sample data to your S3 bucket. Don't worry about trying to send a lot of data now, you will ingest a lot of sample data to your bucket in the next step. 

20. Stop playing the game and check out the contents of your S3 bucket. You should see data in there that looks similar to this:

<p align="center"><img src="img/8.png" /></p> 

21. **Download** one of the files by clicking on it and hitting download. Take a look at the contents. You should see your game data in JSON format like below:

<p align="center"><img src="img/9.png" /></p> 

Congratulations! You finished the first task. You successfully created an S3 data lake and integrated it with a Unity game using the AWS SDK .NET.

<a id="Task2"></a>
[[Top](#Top)]

## Task 2: Populating Data Lake with Amazon Kinesis Data Firehose

Now it's time to ingest a ton of sample data into your S3 data lake using Amazon Kinesis Data Firehose. This section of the lab is to give you practice getting hands-on with Amazon Kinesis as well as ingest a ton of sample data so that you have more to work with and analyze. 

22. The first step is to create a Kinesis Data Firehose stream. In the AWS Management Console, go to Services and click **Kinesis**.

23. In the top right corner, you will see a section for Kinesis Firehose delivery streams. Click **Create delivery stream**.

24. Give your stream a name. This lab will use the name _serverless-games-stream_. 

25. Under Choose source, keep it the default as _Direct PUT or other sources_. For now, you will use a different data source that we will configure shortly, but in practice you can make direct API calls to Kinesis Firehose from your game client or server to send near real time streaming data.

26. Hit **Next**.

27. Leave these configurations as default and hit **Next** again.

28. Under Select destination, make sure **Amazon S3** is selected.

29. On the same page under S3 destination, choose the S3 bucket you created in the first task. 

30. You can take a look at all the other configuration options and explore them if you want but for now leave them all default and hit **Next**.

31. Scroll down to IAM role. This is the Identity and Access Manamgenent role that you need to specify to give Kinesis the appropriate permissions it needs to access your S3 bucket and any other resources it may need. Click **Create new or choose**.

32. This will open up a new tab like the one below where you can create a new IAM role. Select **Allow**.

<p align="center"><img src="img/10.png" /></p> 

33. You should be redirected back to the tab where you are configuring your Kinesis Firehose stream. Review your configuration settings and finally select **Create delivery stream**. You should now see your newly created stream on the Kinesis Firehose dashboard. It will take a minute or two to create, but once it says the status is active you can click into it to find details. 

34. Now we need to have a data source. For this, you will use the **Amazon Kinesis Data Generator** found at this link: https://awslabs.github.io/amazon-kinesis-data-generator/web/producer.html. Open this link - you should see a webpage similar to the one below. 

<p align="center"><img src="img/11.png" /></p> 

35. Click the **Help** button. This page will walk you through how to configure Kinesis Data Generator with your AWS account. Follow the steps to **Create a Cognito User with CloudFormation**.

36. This will bring you to the AWS Management Console CloudFormation dashboard. You should see the followng:

<p align="center"><img src="img/12.png" /></p> 

37. Hit **Next**. Create a username and password that will be used to sign into the Kinesis Data Generator. Hit **Next** again.

38. Leave all other configuration default and hit **Next** twice more. 

39. Review your configurations and check the box at the bottom that acknowledges that this CloudFormation template might create IAM sources. Finally click **Create stack**. It will take a few minutes for the CloudFormation stack to finish creating.

40. When it is finished, on the _Outputs_ page of the stack you will see a **KinesisDataGeneratorUrl**. Open this link. You will use this link to sign in to a page that looked the same before. Sign in using the username and password you just configured in the CloudFormation template. 

41. When the log-in is successful, you will see some fields that you can start configuring to send sample data to populate your S3 data lake. 

      * Set the **Region** as the same one you created your Kinesis Firehose stream in.
      * Set the **Stream** as the one you just created.
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

<p align="center"><img src="img/13.png" /></p> 

42. Hit **Send data**. You should see data starting to send. Let the data generator send a couple thousand records (3000 records for example would be a good stopping point) and then finally hit **Stop sending data to Kinesis**. 

43. Go back to the AWS Management Console tab with your Kinesis Firehose delivery stream open. Click into the stream to view the stream details if you are not viewing them already.

44. Select the **Monitoring** tab. It takes about 1 minute for your stream to start recieving data. Wait a little bit and then refresh a couple times to see metrics start populating, like below:

<p align="center"><img src="img/14.png" /></p> 

45. Go back to your S3 bucket to look at the contents and **verify** the data has been delivered. 

Congratulations! You have successfully created a Kinesis Data Firehose stream and used it to ingest sample records into your S3 data lake!


<a id="Task3"></a>
[[Top](#Top)]

## Task 3: Using AWS Glue to Discover Data

Now that you have all of the data you want to analyze in your S3 data lake, it is time to discover that data and make it available to be queried. 

46. In the AWS Management Console, go to Services, and click **AWS Glue**. 

47. On the left-side navigation bar, select **Crawlers**. You are creating a Glue Crawler, which will _crawl_ through the data in your S3 bucket. It is going to connect to your S3 data store and classify it to determine the schema and metadata. 

48. Select **Add crawler**.

49. Enter a crawler name. For this lab, _serverless-games-crawler_ will be used as the name.

50. Select **Next**. Leave the crawler source type as default, which should be Data stores. Select **Next**.

51. Now, you choose the data store you want to crawl through. It should be defaulted to S3, but you still need to specify the bucket and path of the data you want to discover. Select the **folder icon** to navigate to the path that the Kinesis Data Generator data is at.

<p align="center"><img src="img/15.png" /></p> 

52. Find your bucket, click the **+** next to it, and keep navigating inside the sub folders until you find the file that contains the sample data. Select the folder that file is in, similar to this:

<p align="center"><img src="img/16.png" /></p> 

53. Hit **Select** and then **Next**. Do not add another data store - continue hitting **Next**

54. As you did when you created the Kinesis Data Firehose stream, you need to create an IAM role for your Glue crawler to allow it permissions to access resources it might need access to. Create a role, give it a name, and select **Next**.

55. Keep the Frequency to run on demand and select **Next**.

56. On the page where you define a database, select **Add database** and give it a name. This lab will use the name _serverless-catalog_. Hit **Create** and then **Next**.

<p align="center"><img src="img/17.png" /></p> 

* You just created a Glue Data Catalog, which contains references to your data in S3. It is an index to the location, schema, and runtime metrics of your data and is populated by the Glue crawler. 

57. Review your configurations and select **Finish** to create the crawler. 

58. You should be redirected to AWS Glue dashboard. Find the crawler you just created, select it, and hit **Run crawler**. It might take a few minutes for your crawler to run, but when it is done it should say that a table has been added. Wait for your crawler to finish running.

59. On the left-side navigation bar, select **Databases**. You should see the Glue Data Catalog that you have created. Select it and then click the link to view the tables in your catalog. 

<p align="center"><img src="img/18.png" /></p> 

* You should see a table if you click into it you will see information about the table and the schema of it like below:

<p align="center"><img src="img/19.png" /></p> 

Congratulations! You have successfully used AWS Glue to create a crawler and populate a Glue Data Catalog to discover the data in your S3 data lake.


<a id="Task4"></a>
[[Top](#Top)]

## Task 4: Querying Data with Amazon Athena

Now that you have made your data discoverable, you can query your data for exactly what you want to analyze. 

60. Go to AWS Management Console, click Services, then select **Athena**.

61. On the left hand side under Database, select the **Glue Data Catalog** you just created. In the case of the lab, it is the _serverless-catalog_.

62. Create a new **query** in the middle pane of the Athena Dashboard. Here you can write standard SQL to run queries against your Glue Data Catalog. Run the following query:

```
SELECT * FROM "serverless-catalog"."23" WHERE "serverless-catalog"."23"."time played" > 3600
```

* You might need to change where it says _serverless-catalog_ to the name of your own Glue Data Catalog. You also might need to change the table name to match the name of yours, which you can find in the left-hand Database pane. For reference, the table name here is "23" so change this to match your table name. Here we are querying all the data where the players had spent more than 3600 seconds, or 1 hour, playing the game. 

* Your final result should look like this:

<p align="center"><img src="img/20.png" /></p> 

63. After you run the query and the results display on the dashboard, click **Create** and then **Create view from query**. Give it a name. This lab uses _data-view_ as the name. Click **Create**. 

Congratulations! You have finished querying your data using Amazon Athena. 

<a id="Task5"></a>
[[Top](#Top)]

## Task 5: Discovering Insights with Amazon QuickSight

Now that you have queried for a subset of your data, it is time to analyze it using Amazon QuickSight.

64. Go to the AWS Management Console, select Services, and then choose **QuickSight**.

65. Before importing your data and creating visualizations, you need to make sure that certain permissions are in order. Change the **region** in the top right corner to US East (N. Virginia) as this is the only region that you can currently edit QuickSight settings in. 

66. In the top right, next to where you just changed the region, select your user and then choose **Manage QuickSight** from the drop down. 

67. On the left-hand panel, select **Security & permissions**. You need to allow QuickSight access to certain AWS services. Click **Add or remove** to edit the services QuickSight has permissions to access. Make sure both S3 and Athena are enabled.

<p align="center"><img src="img/22.png" /></p> 

68. After ensuring the correct permissions, go back to the region you were working in before if it is not US East 1, since this is the region you have been working in the entire lab and will be where your Glue Data Catalog and your Athena table view are. 

69. Once you are on the QuickSight dashboard, select **New analysis** in the top left corner. 

70. Select **New data set** and then click **Athena**. You are going to use the table view you just created as the data set for your QuickSight visualizations, to only analyze player data for those who played more than 1 hour. 

71. Enter a name for your data source. This lab will use _serverless-games-data-source_ as the name. Click **Create data source**.

72. Next, it says to choose your table. Select the **Glue Data Catalog** you queried in Athena. Then, select the **table view** you just created using Athena in the last task. It should look similar to this:

<p align="center"><img src="img/21.png" /></p> 

73. Hit **Select**. Next, you can choose to **Import to SPICE for quicker analytics** if you want to do so. SPICE is QuickSights in-memory calculation engine that improves the performance of importing data and creating visualizations. Select **Visualize**.

74. Once your data has been imported, you can start messing around with QuickSight to see what visualizations you can create. Select the **+ ADD** button on the top left to add a visualization. 

75. Under Visual types, select a **Line chart** visualization. 

76. Drag the **time played** variable to be the value for the X axis.

77. Drag the wins and losses variables to be the green values. Click the drop down arrow next to both of these variables to sort them 

78. After adding all three variables, click the drop down arrow next to the time played variable to sort it to be **ascending** by time played. You should end up with something that looks similar to this:

<p align="center"><img src="img/23.png" /></p> 

* This graph shows the average losses and average wins over time. In this case, the data is randomized, but in practice a graph like this could indicate that if your players are winning too much, the game might be too easy and they might get bored and stop playing. On the other hand, it could indicate that if your players are losing too often, the game might be too hard, and as a result the players could get frustrated and stop playing. 

79. You can also add filters to you graph. On the left-hand panel, select **Filter** and create one for **playerid**. Click on **playerid** to expand it.

80. Hit **Apply**. Your final result should look something like this:

<p align="center"><img src="img/24.png" /></p> 

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






