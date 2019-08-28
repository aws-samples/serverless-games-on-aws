## Serverless Games on AWS

Learn how to build serverless games on AWS by integrating the AWS SDK for .NET with Unity.

This is a repository filled with samples and hands-on labs to enable developers to create serverless games on AWS using services like Amazon Cognito, Amazon API Gateway, AWS Lambda, Amazon DynamoDB, and more. Below is a sample architecture for building out a serverless mobile or web game:

<p align="center"><img src="img/Serverless%20Games.png" /></p>


These labs will focus on building out different aspects of this architecture. Unity and the AWS SDK for .NET are used for these tutorials, however the concepts can be applied to other game engines. 

1. You can use **Amazon API Gateway** to add an additional layer between your mobile users and your logic and data stores. It allows backend logic to be interchanged without modifications in the game client. This can be used along with WebSockets to make API calls to AWS services to add functionality to your game. For example, you can access **Amazon Cognito** through API Gateway to add authentication and authorization to your game. 
2. You can use services like **AWS Lambda** and **Amazon DynamoDB** on the backend of your game. For example, Lambda can handle backend logic, like processing requests from the game client for updating player data or a leaderboard that is being stored in DynamoDB.  
3. You can add a serverless analytics pipeline to your game using services like **Amazon S3**, **AWS Glue**, **Amazon Athena**, and **Amazon QuickSight** to discover useful insights that could help attract more players, increase their enjoyment, and get them to play longer. 
4. You can use **Amazon Pinpoint** to engage with your players by sending them targeted emails, SMS, and more. It can help to collect and visualize data to better understand your game usage. 
5. Use **Amazon Simple Notification Service** to send mobile push notifications to players.


## [Cognito User Pools Authentication Lab](https://github.com/aws-samples/serverless-games-on-aws/tree/master/Cognito%20User%20Pools%20Authentication%20Lab)

This lab walks through the process of adding authentication to a Unity game using Cognito User Pools and the AWS SDK for .NET. 

## [Serverless Data Analytics Lab](https://github.com/aws-samples/serverless-games-on-aws/tree/master/Serverless%20Data%20Analytics%20Lab)

This lab walks through the process of sending Unity game logs to an S3 data lake using the AWS SDK .NET. It will explore setting up a serverless analytics pipeline using services like S3, Kinesis, Glue, Athena, and QuickSight to analyze data about game players. 


## License Summary

This sample code is made available under the MIT-0 license. See the LICENSE file.
