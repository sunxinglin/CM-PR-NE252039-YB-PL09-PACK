import request from '@/utils/request'

export function load(params) {
  return request({
    url: '/Flow/Load',
    method: 'get',
    params
  })
}
export function getList(params) {
  return request({
    url: '/Flow/GetList',
    method: 'get',
    params
  })
}

export function getChildList(params)
{
	return request({
	  url: '/Flow/GetFlowStepList',
	  method: 'get',
	  params
	})
	
}

export function add(data) {
  return request({
    url: '/Flow/Add',
    method: 'post',
    data
  })
}

export function update(data) {
  return request({
    url: '/Flow/Update',
    method: 'post',
    data
  })
}

export function del(data) {
  return request({
    url: '/Flow/DeleteEntity',
    method: 'post',
    data
  })
}
export function GetMapByStepId(params)
{
	return request({
	  url: '/Flow/GetMapByStepId',
	  method: 'get',
	  params
	})
	
}
