#!/usr/bin/env python3
from flask import Flask
from flask_restful import Resource, Api
import json
import random

app = Flask(__name__)
api = Api(app)

with open('processed/survey_data.json') as json_file:
    survey_data = json.load(json_file)


with open('processed/mosquito_data.json') as json_file:
    mosquito_data = json.load(json_file)


class Events(Resource):
    def get(self):
        if random.randint(0, 1) == 0:
            rand_int = random.randint(0, len(survey_data))
            lat = survey_data[rand_int]['latitude']
            long = survey_data[rand_int]['longitude']
            text = random.choice(['Diagnosis'])
        else:
            rand_int = random.randint(0, len(mosquito_data))
            lat = mosquito_data[rand_int]['latitude']
            long = mosquito_data[rand_int]['longitude']
            text = random.choice(
                ['Mosquito habitat detected', 'Mosquito larave found']
            )
        events = [{
            'event': {
                'lat': lat,
                'long': long,
                'type': 1,
                'severity': 3,
                'text': text,
            }
        }]
        return(events)


api.add_resource(Events, '/events')


if __name__ == '__main__':
    app.run(port='5002')
