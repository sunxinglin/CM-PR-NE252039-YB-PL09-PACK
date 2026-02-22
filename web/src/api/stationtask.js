import request from '@/utils/request'
export function getList(params) {
    return request({
      url: '/StationTask/Load',
      method: 'get',
      params
    })
  }
  export function add(data) {
    return request({
      url: '/StationTask/Add',
      method: 'post',
      data
    })
  }
  
  export function update(data) {
    return request({
      url: '/StationTask/Update',
      method: 'post',
      data
    })
  }
  export function LoadTaskByProductStepId(params) {
    return request({
      url: '/StationTask/LoadTaskByProductStepId',
      method: 'get',
      params
    })
  }
  
  export function del(data) {
    return request({
      url:  '/StationTask/DeleteEntity',
      method: 'post',
      data
    })
  }
  export function UpTaskOrder(data) {
    return request({
      url:  '/StationTask/UpTaskOrder',
      method: 'post',
      data
    })
  }


  
