import json
import boto3
import base64
import datetime

cloudwatch = boto3.client('cloudwatch')

def lambda_handler(event, context):
    
    output = []
    success = 0
    failure = 0
    
    for record in event['records']:

        print(record)
        bytesArray = base64.b64decode(record['data'])
        my_json = bytesArray.decode('utf8').replace("'", '"')
        data = json.loads(my_json)
        payload = json.dumps(data)
        print(payload)
        playerID = data["PlayerID"]
        wins = data["Wins"]
        losses = data["Losses"]
        timeplayed = data["COL_TimePlayed"]
        caughtat = data["CaughtAt"]
        
        caughtby = data["CaughtBy"]
        print(playerID)
        print(wins)
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
        print(response)
        output.append({'recordId': record['recordId'], 'result': 'Ok'})
        success += 1

    print('Successfully delivered {0} records, failed to deliver {1} records'.format(success, failure))
    return {'records': output}


