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
using System.IO;
using Amazon;
using UnityEngine;
using Amazon.KinesisFirehose;
using Amazon.KinesisFirehose.Model;
using System.Text;
using Newtonsoft.Json;


public class KinesisFirehose : MonoBehaviour
{

    private static AmazonKinesisFirehoseClient client;
    //Define region DeliveryStream is in if it is different than us-west-2
    private static readonly RegionEndpoint streamRegion = RegionEndpoint.USWest2;
    //Define KinesisFirehose delivery stream name here
    private const string streamName = "serverless-games-stream";

    public GameEnding gameEnding;

    private bool sent;
    static Hashtable recordData;

    static int playerid = 1;
    static float timeplayed;
    static int losses;
    static int wins;


    void Start()
    {
        //Create the client to interact with Kinesis
        client = new AmazonKinesisFirehoseClient(streamRegion);
        sent = false;
        recordData = new Hashtable();
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

            _ = WriteRecord();

        }
    }

    static async Task WriteRecord()
    {
        try
        {
            timeplayed = Time.time;

            recordData.Add("PlayerID", playerid);
            recordData.Add("Time Played", timeplayed);
            recordData.Add("Losses", losses);
            recordData.Add("Wins", wins);

            //Convert Hashtable into Json and subsequent byte array in preparation for record
            byte[] dataAsBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(recordData));

            //Create a PutRecordRequest
            PutRecordRequest putRecordRequest = new PutRecordRequest
            {
                DeliveryStreamName = streamName,
                Record = new Record
                {
                    Data = new MemoryStream(dataAsBytes)
                }
            };

            // Put record into the DeliveryStream
            PutRecordResponse response = await client.PutRecordAsync(putRecordRequest);
        }
        catch (AmazonKinesisFirehoseException e)
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


