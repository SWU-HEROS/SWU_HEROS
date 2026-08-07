from fastapi import APIRouter
from crud.db_crud import read_data
import time

router = APIRouter()


@router.get('/first')
async def frist_preprocess():
    # 1. Record the start time
    start_time = time.time()
    print(f"[level 1] Start time: {start_time}")

    # 2. Read data from the database
    data = await read_data()

    # 3. Record the end time
    end_time = time.time()
    print(f"[level 1] End time: {end_time}")

    # 4. Calculate the execution time
    execution_time = end_time - start_time
    print(f"[level 1] Execution time: {execution_time} seconds")

    return  data
