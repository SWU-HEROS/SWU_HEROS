from dataGenerator.db.mongo import connect_mongo
from dataGenerator.crud.people_data import * 
from dataGenerator.utils.make_new_doc import * 

import logging

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s"
)

def get_off_person():
    
    collection_name = 'people_data'
    collection = connect_mongo(collection_name)
    
    #이전 문서 가져오기 
    prev_doc = get_latest_document(collection)
    
    #이전문서 바탕으로 새 문서 만들기 
    new_doc = create_next_document_get_off(prev_doc, -1.11, 6.03)
    
    #새 문서 db에 넣기
    result = create_document(collection, new_doc)
    
    logging.info(result)


def get_on_person():
    
    collection_name = 'test_collection'
    collection = connect_mongo(collection_name)
    
    #이전 문서 가져오기 
    prev_doc = get_latest_document(collection)
    
    #이전문서 바탕으로 새 문서 만들기 
    new_doc = create_next_document_get_on(prev_doc, 1.11, -6.03)
    
    #새 문서 db에 넣기
    result = create_document(collection, new_doc)
    
    logging.info(result)
    

def main():
    get_off_person()
    get_on_person()


if __name__=="__main__":
    
    logging.info('start')
    
    
