import request from '@/utils/request'

export function Load(params) {
  return request({
    url: '/Proc_Product_In_Statistics/load',
    method: 'get',
    params
  })
}