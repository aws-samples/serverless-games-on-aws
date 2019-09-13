/*
 * Copyright 2019 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * SPDX-License-Identifier: MIT-0
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this
 * software and associated documentation files (the "Software"), to deal in the Software
 * without restriction, including without limitation the rights to use, copy, modify,
 * merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections;
using System.Threading.Tasks;
using Amazon;
using UnityEngine;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;

public class S3 : MonoBehaviour
{

  
    private static string keyName;
    private static IAmazonS3 client;
    //define region bucket is in if it is different than us-west-2
    private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USWest2;
    //define S3 bucket name here
    private const string bucketName = "";

    public GameEnding gameEnding;
    
    private bool sent;
    static Hashtable data;

    static int playerid = 1;
    static float timeplayed;
    static int losses;
    static int wins;


    void Start()
    {
        client = new AmazonS3Client(bucketRegion);
        sent = false;
        data = new Hashtable();
        
    }

    void Update()
    {
        if ((gameEnding.isPlayerAtExit() || gameEnding.isPlayerCaught()) && !sent)
        {
            if (gameEnding.isPlayerCaught())
                losses += 1;
            if (gameEnding.isPlayerAtExit())
                wins += 1;

            sent = true;
            _ = WritingAnObjectAsync();
    
        }
    }

    static async Task WritingAnObjectAsync()
    {
        try
        {
            keyName = DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year +
                "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;

            timeplayed = Time.time;

            data.Add("PlayerID", playerid);
            data.Add("Time Played", timeplayed);
            data.Add("Losses", losses);
            data.Add("Wins", wins);

            // 1. Put object-specify only key name for the new object.
            var putRequest1 = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                ContentBody = JsonConvert.SerializeObject(data),
                ContentType = "application/json"
            };
   
            PutObjectResponse response1 = await client.PutObjectAsync(putRequest1);
        }
        catch (AmazonS3Exception e)
        {
            Debug.Log(
                    "Error encountered ***. Message:'{0}' when writing an object" + e);
        }
        catch (Exception e)
        {
            Debug.Log(
                "Unknown encountered on server. Message:'{0}' when writing an object" + e);
        }
    }
}

