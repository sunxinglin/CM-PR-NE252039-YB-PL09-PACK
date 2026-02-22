import request from '@/utils/request'
export function add(data) {
    return request({
      url: '/AGV/Add',
      method: 'post',
      data
    })
  }
  
  export function update(data) {
    return request({
      url: '/AGV/Update',
      method: 'post',
      data
    })
  }
  
  export function del(data) {
    return request({
      url: '/AGV/DeleteEntity',
      method: 'post',
      data
    })
  }
  export function load(data) {
    return request({
      url: '/AGV/Load',
      method: 'post',
      data
    })
  }  
  export function BingAgv(data) {
    return request({
      url: '/AGV/BingAgv',
      method: 'post',
      data
    })
  }  
  export function AGVArrived(data) {
    return request({
      url: '/AGV/AGVArrived',
      method: 'post',
      data
    })
  } 

  export function AGVLeaved(data) {
    return request({
      url: '/AGV/AGVLeaved',
      method: 'post',
      data
    })
  }  
  export function runAGV(params) {
    return request({
      url: '/AGV/RunAGV',
      method: 'get',
      params
    })
  }  