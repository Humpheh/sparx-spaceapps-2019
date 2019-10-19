#!/usr/bin/env python3
from flask import Flask
from flask_restful import Resource, Api
import json
import random
import numpy as np
from shapely.geometry import Point
import geopandas as gpd

app = Flask(__name__)
api = Api(app)

with open('processed/survey_data.json') as json_file:
    survey_data = json.load(json_file)

with open('processed/mosquito_data.json') as json_file:
    mosquito_data = json.load(json_file)

with open('processed/malaria_polygon.json') as geojson_file:
    malaria_polygon = gpd.read_file("processed/malaria_polygon.json").iloc[0].geometry

raster_map = np.genfromtxt(
    "processed/raster-world-map.txt",
    dtype=np.uint8,
    delimiter=1
)

def random_location():
    lat = random.uniform(0, 159)
    long = random.uniform(0, 359)
    return np.round(np.array([lat, long]), 5)

def inland(lat, long):
    return raster_map[int(lat), int(long)] == 1

def in_malaria_area(lat, long):
    return malaria_polygon.contains(Point(lat, long))

def random_land_location():
    while True:
        loc = random_location()
        lat, long = loc
        if inland(lat, long): return loc

def sample_real_event():
    return random.sample(mosquito_data, 1)[0]

def new_location_nearby(lat, long):
    N = 10
    n = 1
    while True:
        n += 1
        new_lat = lat + random.uniform(-10, 10)
        new_long = long + random.uniform(-20, 20)
        if inland(new_lat, new_long):
            return np.round(np.array([new_lat, new_long]), 5)
        if n > N:
            return random_land_location()

def new_timer(lifestage):
    if lifestage == 'adults':
        return 15
    elif lifestage == 'pupae':
        return 30
    else:
        return 60

def event():
    event = sample_real_event()
    lat = event['latitude']
    long = event['longitude']
    events = [{
        'event': {
            'lat': event['latitude'],
            'long': event['longitude'],
            'timer': new_timer(event['severity']),
            'text': event['text'],
            'image_url': event['image_url'],
            'malarial_risk': in_malaria_area(lat, long),
        }
    }]
    return events

def spread(lat, long):
    n_spread = random.randint(1, 4)
    events = []
    for i in range(0, n_spread):
        new_loc = new_location_nearby(lat, long)
        events.append({
            'lat': new_loc[0],
            'long': new_loc[1],
            'timer': random.uniform(40, 60),
            'text': 'Mosquito activity spreading!',
            'image_url': None,
            'malarial_risk': False
        })
    return events

class Events(Resource):
    def get(self):
        return event()

class Spread(Resource):
    def get(self, lat, long):
        return spread(float(lat), float(long))


api.add_resource(Events, '/events/new')
api.add_resource(Spread, '/events/spread/<lat>/<long>')


if __name__ == '__main__':
    app.run(port='5002')
