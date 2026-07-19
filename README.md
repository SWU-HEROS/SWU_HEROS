# PrivCrowd Twin

The **3D Digital Twin-Based Crowd Management System** is an analytical tool that uses real-world data to simulate crowd congestion and provides AI-based future crowd prediction and personal data de-identification capabilities.

## Key Features (Main Features)

* **Subway Passenger Data Training**: The system collects near-real-time data from telecommunications sources and subway passenger data from the Seoul Open Data Plaza, and trains a Prophet model through cross-validation using data from 2022 to 2024.
* **Machine Learning-Based Crowd Data Generation**: Using the trained model, the system predicts future passenger counts for the 3D simulation based on factors such as the current time, month, day of the week, and whether it is Halloween.
* **Selective Personal Data De-identification**: The system classifies data as personal, pseudonymized, or anonymized information and performs de-identification in real time when an API call occurs within the 3D digital twin, thereby protecting privacy.
* **3D Simulation-Based Crowd Density Visualization**: The system visualizes a 3D environment identical to the real-world environment by dividing it into $1m^2$ cells. A cell containing eight or more people is classified as 'Very Dangerous,' allowing risk areas to be identified intuitively.

## Installation Guide (Installation)

### 1. Clone the Repository (Clone the repository)
```bash
git clone [https://github.com/SWU-HEROS/SWU_HEROS.git](https://github.com/SWU-HEROS/SWU_HEROS.git)
cd SWU_HEROS
```

### 2. Install Dependencies (Install dependencies)
* **Unity**: Unity 2022.3.28f1 LTS is required.
* **Python**: Install the backend dependencies using the following commands.
```bash
cd dataGenerator
pip install requirements.txt

cd ..
cd predictAPI
pip install requirements.txt
```

## How to Use (Usage)
1. **Database Setup**: Run MongoDB and import the collected subway boarding and alighting data.
2. **Run the Backend Server**: Activate the de-identification and prediction APIs through FastAPI.
```bash
uvicorn main:app --reload
```
3. **Run the Simulation**: Open the Unity project and select either Monitoring or Simulation mode.
4. **View the Results**:
   * Check the cell-by-cell color changes on the digital twin screen.
   * After the simulation ends, review the report showing the total elapsed time and the number of station-skipping operations.

## Project Organization (Project Structure)
```text
SWU-HEROS/
├── dataGenerator/              # Generates information for each object stored in MongoDB
├── predictAPI/                 # Predicts subway boarding and alighting passenger counts
├── pseudonymousProcessingAPI/  # API for applying selective personal data de-identification
├── simul_people/               # Updates the coordinates of each object during the simulation
└── unitySystem/                # 3D digital twin system
```
## Data De-identification Model (De-identification Model)
* **Level 1 (Identifiable Personal Information Level)**: Includes all information, such as phone numbers and IMSIs.
* **Level 2 (Partially Masked Personal Information Level)**: Partially masks phone numbers (010-****-5432), removes IMSIs, and groups ages.
* **Level 3 (Fully Masked Personal Information Level)**: Completely removes phone numbers and IMSIs and broadly generalizes age groups (e.g., Youth and Senior).

## License (License)
This project is distributed under the MIT License.
