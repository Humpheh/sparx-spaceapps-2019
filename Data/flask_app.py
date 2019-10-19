from flask import Flask, request
from flask_restful import Resource, Api
import json
import random

app = Flask(__name__)
api = Api(app)

with open('processed/survey_data.json') as json_file:
    data = json.load(json_file)

class Events(Resource):
    def get(self):
        rand_int = random.randint(0, len(data))
        lat = data[rand_int]['latitude']
        long = data[rand_int]['longitude']
        events = [{'event': {'lat': lat, 'long': long, 'type': 1, 'severity': 3, 'text': 'Reports of 23 people diagnosed with Malaria'}}]
        return(events)

api.add_resource(Events, '/events')


if __name__ == '__main__':
     app.run(port='5002')
