import React, { useContext } from "react";
import { AuthContext } from "../../setting/AuthContext";
import defaultAvatar from "../../assets/img/defaultAvatar.jpg";
import { useNavigate } from "react-router-dom";

export default function ProfilePage() {
  const { userInfor, setIsAuthenticated, setUserInfor } =
    useContext(AuthContext);
  const nav = useNavigate();
  const logout = () => {
    setIsAuthenticated(false);
    setUserInfor(null);
    nav("/");
  };

  return (
    <div className="w-full max-w-4xl mx-auto p-6 bg-white shadow-lg rounded-lg mt-10">
      <div className="flex items-center space-x-6">
        {/* User Avatar */}
        <img
          src={userInfor?.picture || defaultAvatar}
          alt="User Avatar"
          className="w-24 h-24 rounded-full object-cover border-2 border-blue-500"
        />
        {/* User Information */}
        <div className="flex flex-col">
          <h1 className="text-2xl font-bold text-gray-900">
            {userInfor?.name || "John Doe"}
          </h1>
          <p className="text-gray-600">
            {userInfor?.email || "johndoe@example.com"}
          </p>
          <p className="text-blue-500">{userInfor?.role || "User Role"}</p>
        </div>
      </div>

      {/* Additional Profile Information */}
      <div className="mt-6">
        <h2 className="text-xl font-semibold text-gray-800">
          Profile Information
        </h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mt-4">
          {/* Example fields */}
          <div>
            <label className="block text-gray-700 font-semibold mb-2">
              Phone Number
            </label>
            <p className="bg-gray-100 p-2 rounded-lg text-gray-800">
              {userInfor?.phone || "Not provided"}
            </p>
          </div>
          <div>
            <label className="block text-gray-700 font-semibold mb-2">
              Address
            </label>
            <p className="bg-gray-100 p-2 rounded-lg text-gray-800">
              {userInfor?.address || "Not provided"}
            </p>
          </div>
          <div>
            <label className="block text-gray-700 font-semibold mb-2">
              Joined On
            </label>
            <p className="bg-gray-100 p-2 rounded-lg text-gray-800">
              {userInfor?.joined || "01 Jan 2023"}
            </p>
          </div>
        </div>
      </div>

      {/* Edit Profile and Logout Buttons */}
      <div className="flex space-x-4 mt-8">
        <button
          className="bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600"
          onClick={() => nav("../editProfile")}
        >
          Edit Profile
        </button>
        <button
          className="bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600"
          onClick={logout}
        >
          Log Out
        </button>
      </div>
    </div>
  );
}
