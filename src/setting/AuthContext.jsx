import { createContext, useEffect, useState } from "react";

export const AuthContext = createContext();
export function AuthProvider({ children }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userInfor, setUserInfor] = useState(() => {
    const storedData = localStorage.getItem("UserInfor");
    return storedData ? JSON.parse(storedData) : null;
  });
  useEffect(() => {
    localStorage.setItem("UserInfor", JSON.stringify(userInfor));
  }, [userInfor]);

  return (
    <AuthContext.Provider
      value={{ isAuthenticated, setIsAuthenticated, userInfor, setUserInfor }}
    >
      {children}
    </AuthContext.Provider>
  );
}
