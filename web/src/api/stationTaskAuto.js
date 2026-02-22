import request from '@/utils/request'

export function add(data) {
  return request({
    url: '/StationTaskAuto/Add',
    method: 'post',
    data
  })
}

export function del(data) {
  return request({
    url: '/StationTaskAuto/Delete',
    method: 'post',
    data
  })
}

export function update(data) {
    return request({
      url: '/StationTaskAuto/Update',
      method: 'post',
      data
    })
  }
  
export function GetByTaskId(params) {
    return request({
      url: '/StationTaskAuto/GetByTaskId',
      method: 'get',
      params
    })
  }