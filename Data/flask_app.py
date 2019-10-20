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
    lat = random.uniform(50, 130) # exclude the high latitudes
    long = random.uniform(0, 359)
    return np.round(np.array([lat, long]), 5)

def inland(lat, long):
    return raster_map[int(lat), int(long)] == 1

def in_malaria_area(lat, long):
    # malaria polygon is in real long-lat coordinates
    point = Point(long - 180, -(lat - 90))
    return malaria_polygon.contains(point)

def random_land_location():
    while True:
        loc = random_location()
        lat, long = loc
        if inland(lat, long): return loc

def sample_real_event():
    event = random.sample(mosquito_data, 1)[0]
    event['type'] = "mosquito_report"
    event['debug'] = "genuine_event"
    return event

def create_fake_event():
    event = sample_real_event()
    lat, longitude = random_land_location()
    event['latitude'] = lat
    event['longitude'] = longitude
    event['type'] = "mosquito_report"
    event['debug'] = "fake_event"
    return event

def new_event(pReal = 0.2):
    real = random.uniform(0, 1) < pReal
    method = sample_real_event if real else create_fake_event
    return method()

def new_location_nearby(lat, longitude):
    N = 100
    n = 1
    while True:
        n += 1
        new_lat = (lat + random.uniform(-5, 5)) % 180
        new_long = (longitude + random.uniform(-10, 10)) % 360
        if inland(new_lat, new_long):
            return np.round(np.array([new_lat, new_long]), 5)
        if n > N:
            return random_land_location()

def new_timer(lifestage):
    if lifestage == 'adults':
        return random.uniform(10, 20)
    elif lifestage == 'pupae':
        return random.uniform(20, 40)
    else:
        return random.uniform(40, 60)

def event():
    event = new_event()
    lat = event['latitude'] % 180
    long = event['longitude'] % 360
    events = {
        'events': [
            {
                'lat': lat,
                'long': long,
                'timer': new_timer(event['severity']),
                'text': event['text'],
                'image_url': event['image_url'],
                'infection_risk': in_malaria_area(lat, long),
                'type': event['type'],
                'debug': event['debug']
            }
        ]
    }
    print(events)
    return events

def spread(lat, long):
    orig_infection_risk = in_malaria_area(lat, long)
    if orig_infection_risk:
      n_spread = random.randint(3, 6)
    else:
      n_spread = random.randint(1, 2)
    events = []
    for _ in range(0, n_spread):
        new_loc = new_location_nearby(lat, long)
        infection_risk = in_malaria_area(new_loc[0], new_loc[1])
        type = "outbreak" if infection_risk else "mosquito_report";
        text = "Outbreak of Malaria!" if infection_risk else  "Mosquito activity spreading";
        events.append({
            'lat': new_loc[0],
            'long': new_loc[1],
            'timer': random.uniform(40, 60),
            'text': text,
            'image_url': None,
            'infection_risk': infection_risk,
            'type': type,
        })
    return {'events': events}

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
