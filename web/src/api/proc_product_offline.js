import request from '@/utils/request'

export function Load(params) {
  return request({
    url: '/Proc_Product_OffLine/load',
    method: 'get',
    params
  })
}
export function GetRoundCakeData(data) {
  return request({
    url: '/Proc_Product_OffLine/GetRoundCakeData',
    method: 'post',
    data
  })
}

export function GetBrokenLineData(data) {
  return request({
    url: '/Proc_Product_OffLine/GetBrokenLineData',
    method: 'post',
    data
  })
}
export function GetListByProductId(data) {
  return request({
    url: '/Proc_Product_OffLine/GetListByProductId',
    method: 'post',
    data
  })
}