import Cookies from 'js-cookie'

const TokenKey = 'X-Token'

export function getToken() {
  return Cookies.get(TokenKey)
}

export function setToken(token) {
  return Cookies.set(TokenKey, '123456')
}


export function removeToken() {
  return Cookies.remove(TokenKey)
}

export function setUserName(un)
{
	  return Cookies.set("SET_NAME",un)
	
}

export function getUserName() {
  return Cookies.get("SET_NAME")
}

export function setUserAccount(un)
{
	  return Cookies.set("SET_ACCOUNT",un)
	
}

export function getUserAccount() {
  return Cookies.get("SET_ACCOUNT")
}


