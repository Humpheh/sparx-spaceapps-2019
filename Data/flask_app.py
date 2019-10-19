#!/usr/bin/env python3
from flask import Flask
from flask_restful import Resource, Api
import json
import random

app = Flask(__name__)
api = Api(app)

with open('processed/mosquito_data.json') as json_file:
    mosquito_data = json.load(json_file)

class Events(Resource):
    def get(self):
        sample = mosquito_data[random.randint(0, len(mosquito_data))]
        text = sample['text']
        lat = sample['latitude']
        long = sample['longitude']
        image_url = sample['image_url']
        events = [{
            'event': {
                'lat': lat,
                'long': long,
                'type': 1,
                'severity': 3,
                'text': text,
                'image_url': image_url,
            }
        }]
        return(events)


api.add_resource(Events, '/events')


if __name__ == '__main__':
    app.run(port='5002')
