from pymongo.collection import Collection

from bson import ObjectId
from typing import Optional 


# Retrieve the previous data
def get_latest_document(collection: Collection):
    return collection.find_one(sort=[("datetime", -1)])


def create_main_doc(collection: Collection, doc):
    
    #doc["_id"] = ObjectId()
    result = collection.insert_one(doc)
    return str(result.inserted_id)

# Insert the new data into the database
def create_document(collection: Collection, doc: dict):
    
    result = collection.insert_one(doc)
    return str(result.inserted_id)
