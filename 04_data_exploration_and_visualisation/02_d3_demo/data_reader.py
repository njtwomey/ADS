# coding=utf-8
# The MIT License (MIT)
# Copyright (c) 2014-2017 University of Bristol
# 
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
# 
# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.
# 
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
# EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
# MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
# IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
# DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
# OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
# OR OTHER DEALINGS IN THE SOFTWARE.

import pandas as pd
import os


def get_kaggle_data(data_path='kaggle_data'):
    gender_age_train = pd.read_csv(os.path.join(data_path, 'gender_age_train.csv'))
    events = pd.read_csv(os.path.join(data_path, 'events.csv'))
    phone_brand_device_model = pd.read_csv(os.path.join(data_path, 'phone_brand_device_model.csv'))

    df = gender_age_train.merge(events, how='left', on='device_id')
    df = df.merge(phone_brand_device_model, how='left', on='device_id')

    top_10_brands_en = {
        '华为': 'Huawei', '小米': 'Xiaomi', '三星': 'Samsung', 'vivo': 'vivo', 'OPPO': 'OPPO',
        '魅族': 'Meizu', '酷派': 'Coolpad', '乐视': 'LeEco', '联想': 'Lenovo', 'HTC': 'HTC'
    }

    df['phone_brand_en'] = df['phone_brand'].apply(
        lambda phone_brand: top_10_brands_en[phone_brand] if (phone_brand in top_10_brands_en) else 'Other')

    df['age_segment'] = df['age'].apply(lambda age: get_age_segment(age))

    return df


def get_age_segment(age):
    if age < 20:
        return '20-'
    elif age < 25:
        return '20-24'
    elif age < 30:
        return '25-29'
    elif age < 35:
        return '30-34'
    elif age < 40:
        return '35-39'
    else:
        return '40+'

