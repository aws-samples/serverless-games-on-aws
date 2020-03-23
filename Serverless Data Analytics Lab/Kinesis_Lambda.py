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