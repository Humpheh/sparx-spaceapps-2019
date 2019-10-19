from flask import Flask, request
from flask_restful import Resource, Api
from json import dumps

app = Flask(__name__)
api = Api(app)

class Events(Resource):
    def get(self):
        return {'event': {'lat': 50, 'long': 50}}

        

api.add_resource(Events, '/events')


if __name__ == '__main__':
     app.run(port='5002')
