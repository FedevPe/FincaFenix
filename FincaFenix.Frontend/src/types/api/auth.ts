export interface LoginDTO {
  userName: string;
  password: string;
}

export interface CurrentUserDTO {
  id: string;
  userName: string;
  email: string;
  roles: string[];
  policies: string[];
}
