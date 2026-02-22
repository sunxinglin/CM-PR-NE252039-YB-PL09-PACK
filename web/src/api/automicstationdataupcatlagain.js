import request from '@/utils/request'

export function upCatlAgain(data) {
    return request({
        url: '/AutomicStationDataUpCatlAgain/UpCatlAgain',
        method: 'post',
        data
    })

}

