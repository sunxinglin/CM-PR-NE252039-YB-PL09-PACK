import request from '@/utils/request';

export function getPageList(params) {
  return request({
    url: '/Base_ScrewNGResetConfig/PageList',
    method: 'get',
    params
  })
}

export function add(data) {
  return request({
    url: '/Base_ScrewNGResetConfig/Add',
    method: 'post',
    data
  })
}

export function update(data) {
  return request({
    url: '/Base_ScrewNGResetConfig/Update',
    method: 'post',
    data
  })
}

export function del(data) {
  return request({
    url: '/Base_ScrewNGResetConfig/Delete',
    method: 'post',
    data
  })
}

