import { Outlet } from "react-router-dom";
import { HeaderAuthenticated, HeaderUnauthenticated } from "../layout/Header";

export  function LayoutGuest() {
  return (
    <div>
      <div>
        <HeaderUnauthenticated />
      </div>
      <div>
        <Outlet />
      </div>
    </div>
  );
}
export function LayoutLogined() {
  return (
    <div>
      <div className="absolute w-full h-fit">
        <HeaderAuthenticated />
      </div>
      <div className="pt-[5rem]">
        <Outlet />
      </div>
    </div>
  );
}

