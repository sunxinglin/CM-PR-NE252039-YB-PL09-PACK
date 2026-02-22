import request from '@/utils/request'

export function get(params) {

  return request({
    url: '/DictionaryDetail/Get',
    method: 'get',
    params
  })
}

export function load(params) {
  return request({
    url: '/DictionaryDetail/Load',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/DictionaryDetail/Add',
    method: 'post',
    data
  })
}

export function update(data) {
  return request({
    url: '/DictionaryDetail/Update',
    method: 'post',
    data
  })
}

export function del(data) {
  return request({
    url: '/DictionaryDetail/DeleteEntity',
    method: 'post',
    data
  })
}

export function getListByType(params)
{
	return request({
	  url: '/DictionaryDetail/getListByType',
	  method: 'get',
	  params
	})
}
export function LoadDetail(params)
{
	return request({
	  url: '/DictionaryDetail/LoadDetail',
	  method: 'get',
	  params
	})
}
