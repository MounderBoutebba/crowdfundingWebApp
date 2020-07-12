import { Injectable } from '@angular/core';

@Injectable()
export class LocalStorageService {

  constructor() { }
  getData(key, defaultValue = null) {
    try {
      let value = localStorage && localStorage.getItem(key);
      value = JSON.parse(value);
      return (value != null) ? value : defaultValue;
    } catch (e) {
      console.log(e.message)
      return defaultValue;
    }
  }

  setData(key, value) {
    localStorage && localStorage.setItem(key, JSON.stringify(value));
    return this;
  }

  removeData(key) {
    localStorage && localStorage.removeItem(key);
    return this;
  }

}
