#!/usr/bin/env python3
from flask import Flask
from flask_restful import Resource, Api
import json
import random
import numpy as np

app = Flask(__name__)
api = Api(app)

with open('processed/survey_data.json') as json_file:
    survey_data = json.load(json_file)

with open('processed/mosquito_data.json') as json_file:
    mosquito_data = json.load(json_file)

raster_map = np.genfromtxt(
    "processed/raster-world-map.txt",
    dtype=np.uint8,
    delimiter=1
)

def random_location():
    lat = random.uniform(0, 159)
    long = random.uniform(0, 359)
    return np.round(np.array([lat, long]), 5)

def random_land_location():
    while True:
        loc = random_location()
        lat, long = loc
        if raster_map[int(lat), int(long)] == 1: return loc

def real_event():
    return random.sample(mosquito_data, 1)[0]

class Events(Resource):
    def get(self):
        event = real_event()
        events = [{
            'event': {
                'lat': event['latitude'],
                'long': event['longitude'],
                'type': 1,
                'severity': 3,
                'text': event['text'],
                'image_url': event['image_url'],
            }
        }]
        return(events)


api.add_resource(Events, '/events')


if __name__ == '__main__':
    app.run(port='5002')
