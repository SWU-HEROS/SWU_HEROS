from pymongo.collection import Collection
from bson import ObjectId
from typing import Optional

# Insert the new data into the database
def create_document(collection: Collection, doc: dict):
    
    result = collection.insert_one(doc)
    return str(result.inserted_id)

# Retrieve the previous data
def get_latest_document(collection: Collection):
    return collection.find_one(sort=[("datetime", -1)])


