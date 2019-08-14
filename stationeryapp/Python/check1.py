from flask import Flask,request,jsonify
import pandas as pd
import sklearn
from sklearn.ensemble import RandomForestRegressor
import matplotlib.pyplot as plt
import numpy as np
import io
import random
import json
from flask import Response
from matplotlib.backends.backend_agg import FigureCanvasAgg as FigureCanvas
from matplotlib.figure import Figure


app = Flask(__name__)

df = pd.read_csv('employee_.csv')
df_grp= df.groupby(['Item','MOnth'],as_index=False)
df_grp1= df_grp["Quantity"].agg("sum")
Y = df_grp1['Quantity']
X = df_grp1[['Item','MOnth']]
rdfor = RandomForestRegressor(n_estimators = 200, random_state = 42,max_depth=10,min_samples_leaf=5,criterion='mae')
rdfor.fit(X, Y)

@app.route('/itemmonth/',methods=['POST','GET'])
def func1():
    content = request.json
    item1 = int(content['item'])
    # item = int(item)
    month1 = int(content['month'])
    # month = int(month)
    r1 = func(item1,month1)[0]
    r2 = func(item1,month1)[1] 
    prd = func(item1,month1)[2]
    print(r1)
    print(r2)
    print(prd)
    return jsonify(status="ok", month1=r1, month2=r2,predicted = prd )



def func(item,Month):
    df_q1 = df_grp1.loc[(df_grp1['Item'] == item) & (df_grp1['MOnth'] == Month-1)]
    df_q2 = df_grp1.loc[(df_grp1['Item'] == item) & (df_grp1['MOnth'] == Month-2)]  
    predicts(item,Month)                                       
    return int(df_q1['Quantity']),int(df_q2['Quantity']),int(predicts(item,Month))



def predicts(item2,month2):
    return rdfor.predict([[item2,month2]])


dfd = pd.read_csv('Dept_List.csv')
dfd.dropna(inplace=True)
Yd = dfd['Quantity']
Xd = dfd[['Dept','Month']]
rdford = RandomForestRegressor(n_estimators = 200, random_state = 42,max_depth=10,min_samples_leaf=5,criterion='mae')
rdford.fit(Xd, Yd)




def prd(Depart,Mon):
    df1 = dfd[(dfd.Dept == Depart) & (dfd.Month == Mon-1)]
    q1 = int(df1['Quantity'].iloc[0])
    df2 = dfd[(dfd.Dept == Depart) & (dfd.Month == Mon-2)]
    q2 = int(df2['Quantity'].iloc[0])
    prdict = int(rdford.predict([[Depart,Mon,]]))
    return q1,q2,prdict


@app.route('/deptlist/',methods=['POST','GET'])
def functio():
    content = request.json
    dept = int(content['Dept'])
    print(dept)
    # item = int(item)
    month1 = int(content['month'])
    print(month1)
    # month = int(month)
    r1d = prd(dept,month1)[0]
    r2d = prd(dept,month1)[1] 
    prd1 = prd(dept,month1)[2]
    return jsonify(status="ok", month1=r1d, month2=r2d,predicted = prd1)


if __name__ =='__main__':
    app.run(debug=True)