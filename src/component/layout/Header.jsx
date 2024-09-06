import { useContext, useState } from "react";

import BeShelf from "../../assets/icons/BeShelf.svg";
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../../setting/AuthContext";
import defaultAvatar from "../../assets/img/defaultAvatar.jpg";

export function HeaderUnauthenticated() {
  const nav = useNavigate();
  return (
    <div className="flex items-center justify-between h-20 w-full bg-blue-500 text-white px-4">
      <div className="flex items-center">
        <img src={BeShelf} alt="BeShelf Logo" className="h-24 w-auto" />
      </div>
      <div className="flex space-x-4">
        <div>1</div>
        <div>2</div>
        <div>3</div>
        <div>4</div>
        <div>5</div>
      </div>
      <div className="flex space-x-4">
        <button
          className="bg-white text-blue-500 px-3 py-1 rounded hover:bg-blue-600 hover:text-white transition duration-200"
          onClick={() => nav("/signin")}
        >
          Sign In
        </button>
        <button
          className="bg-white text-blue-500 px-3 py-1 rounded hover:bg-blue-600 hover:text-white transition duration-200"
          onClick={() => nav("/signup")}
        >
          Sign Up
        </button>
      </div>
    </div>
  );
}
export function HeaderAuthenticated() {
  const [openUserInfor, setOpenUserInfor] = useState(false);
  const { isAuthenticated, setIsAuthenticated, userInfor, setUserInfor } =useContext(AuthContext);
  const nav = useNavigate();

  const logout = () => {
    setIsAuthenticated(false);
    setUserInfor(null);
    nav("/");
  };

  

  return (
    <div className="flex items-center justify-between h-20 w-full bg-blue-500 text-white px-4">
      <div className="flex items-center">
        <img src={BeShelf} alt="BeShelf Logo" className="h-24 w-auto" />
      </div>
      <div className="flex space-x-4">
        <div>1</div>
        <div>2</div>
        <div>3</div>
        <div>4</div>
        <div>5</div>
      </div>
      <div className="flex space-x-4">
        <button
          className="bg-white text-blue-500 border rounded-full overflow-hidden hover:bg-blue-600 hover:text-white transition duration-200"
          onClick={() => setOpenUserInfor((pre) => !pre)}
        >
          <img
            src={userInfor?.picture || defaultAvatar}
            className="relative w-10 h-10"
            alt="User Avatar"
          ></img>
        </button>
      </div>
      {openUserInfor && (
        <div className="absolute right-4 mt-2 w-48 bg-white rounded-lg shadow-lg z-10 top-20">
          <ul className="py-2 text-gray-700">
            <li className="px-4 py-2 hover:bg-blue-500 hover:text-white cursor-pointer rounded-t-lg" onClick={()=>nav("profile")}>
              Profile
            </li>
            <li className="px-4 py-2 hover:bg-blue-500 hover:text-white cursor-pointer" onClick={()=>nav("setting")}>
              Settings
            </li>
            <li className="px-4 py-2 hover:bg-blue-500 hover:text-white cursor-pointer">
              Notifications
            </li>
            <li className="px-4 py-2 hover:bg-blue-500 hover:text-white cursor-pointer">
              Help
            </li>
            <li
              className="px-4 py-2 hover:bg-blue-500 hover:text-white cursor-pointer rounded-b-lg"
              onClick={logout}
            >
              Log Out
            </li>
          </ul>
        </div>
      )}
    </div>
  );
}
