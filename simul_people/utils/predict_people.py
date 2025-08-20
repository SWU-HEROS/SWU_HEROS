import requests
from datetime import datetime 

get_on_url = 'http://127.0.0.1:8000/get_on_predict'
get_off_url = 'http://127.0.0.1:8000/get_off_predict'


def make_one_hot(prefix, size, active_idx):
    return {f"{prefix}_{i}": int(i == active_idx) for i in range(1, size + 1)}



def request_get_on_num(payload):
    
    get_on_response = requests.post(get_on_url, json=payload)
    
    if get_on_response.status_code == 200:
        
        get_on_items = get_on_response.json()[0]
        get_on_predicts = get_on_items['yhat']
        
        print(f'예측 승차인원:{get_on_predicts}')
    
    
    return get_on_predicts



def request_get_off_num(payload):
    
    get_off_response = requests.post(get_off_url, json=payload)
    
    if get_off_response.status_code == 200:
        
        get_off_items = get_off_response.json()[0]
        get_off_predicts = get_off_items['yhat']
        
        print(f'예측 하차인원:{get_off_predicts}')
    
    
    return get_off_predicts


def make_model_payload():
    
    dt = datetime.now()
    ds = dt.strftime("%Y-%m-%d %H:%M:%S")
    month = dt.month
    hour = dt.hour
    day_of_week = dt.isoweekday()
    is_weekend = int(day_of_week >= 6)
    is_halloween = int(dt.month == 10 and dt.day == 31)
    
    payload = {
        "ds": ds,
        "is_weekend": is_weekend,
        "is_halloween": is_halloween,
    }
    
    payload.update(make_one_hot("dayofweek", 7, day_of_week))
    payload.update(make_one_hot("month", 12, month))
    payload.update(make_one_hot("hour", 24, hour))  # hour_1 ~ hour_24

    return payload