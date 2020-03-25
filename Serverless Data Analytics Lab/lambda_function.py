import json
import boto3
import base64
import datetime

cloudwatch = boto3.client('cloudwatch')

def lambda_handler(event, context):
    
    for record in event['records']:

        bytesArray = base64.b64decode(record['data'])
        my_json = bytesArray.decode('utf8').replace("'", '"')
        data = json.loads(my_json)
        payload = json.dumps(data)

        playerID = data["playerID"]
        wins = data["Wins"]
        losses = data["Losses"]
        timeplayed = data["TimePlayed"]
        caughtat = data["CaughtAt"]
        caughtby = data["CaughtBy"]

        response = cloudwatch.put_metric_data(
        MetricData = [
            {
                'MetricName': 'playerID',
                'Timestamp': datetime.datetime.now(),
                'Value': playerID,
                'StorageResolution': 1
            },
            {
                'MetricName': 'wins',
                'Timestamp': datetime.datetime.now(),
                'Value': wins,
                'StorageResolution': 1
            },
            {
                'MetricName': 'losses',
                'Timestamp': datetime.datetime.now(),
                'Value': losses,
                'StorageResolution': 1
            },
            {
                'MetricName': 'timePlayed',
                'Timestamp': datetime.datetime.now(),
                'Value': timeplayed,
                'StorageResolution': 1
            },
            {
                'MetricName': 'caughtAt',
                'Timestamp': datetime.datetime.now(),
                'Value': caughtat,
                'StorageResolution': 1
            },
            {
                'MetricName': 'CaughtBy',
                'Timestamp': datetime.datetime.now(),
                'Value': caughtby,
                'StorageResolution': 1
            },

        ],
        Namespace='serverless-analytics-demo'
        )
        
      