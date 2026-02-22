import request from '@/utils/request'

export function update(data) {
    return request({
      url: '/StationTaskResource/Update',
      method: 'post',
      data
    })
  }

  export function Add(data) {
    return request({
      url: '/StationTaskResource/Add',
      method: 'post',
      data
    })
  }

  export function getBoltGunList(params) {
    return request({
      url: '/StationTaskResource/GetBoltGunList',
      method: 'get',
      params
    })
  }
