import { createContext, useState } from "react";

export const RoleContext = createContext();

export function RoleProvider({ children }) {
  const [userRole, setUserRole] = useState(null);
  return (
    <RoleContext.Provider value={{ userRole, setUserRole }}>
      {children}
    </RoleContext.Provider>
  );
}
