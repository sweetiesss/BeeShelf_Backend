import { useContext, useState } from "react";
import { useNavigate } from "react-router-dom";

export default function SignUp() {
  const [form, setForm] = useState({});
  const [agree, setAgree] = useState(false);
  const nav = useNavigate();
  const handleInput = (e) => {
    const value = e.target;
    setForm(() => ({ ...form, [value.name]: value.value }));
  };

  return (
    <div className="w-full max-w-lg p-4 mx-auto bg-white rounded-2xl shadow-md sm:p-6 lg:p-8 relative ">
      <button
        className="absolute left-5 top-2 text-2xl font-bold text-gray-500 hover:text-gray-800"
        onClick={() => nav("/")}
      >
        {"<"}
      </button>
      <header className="mb-4">
        <h1 className="text-2xl font-semibold text-center">Sign Up</h1>
      </header>
      <div className="flex flex-col space-y-4">
        <div>
          <label>Email*</label>
          <input
            className="border border-gray-300 rounded-lg p-2 w-full focus:outline-none focus:ring-2 focus:ring-blue-500 mt-2"
            type="text"
            onChange={handleInput}
            name="Email"
            placeholder="Email"
            value={form?.Email || ""}
          />
        </div>
        <div>
          <label>Full name*</label>
          <input
            className="border border-gray-300 rounded-lg p-2 w-full focus:outline-none focus:ring-2 focus:ring-blue-500 mt-2"
            type="text"
            onChange={handleInput}
            name="FullName"
            placeholder="Full Name"
            value={form?.FullName || ""}
          />
        </div>
        <div>
          <label>Phone number*</label>
          <input
            className="border border-gray-300 rounded-lg p-2 w-full focus:outline-none focus:ring-2 focus:ring-blue-500 mt-2"
            type="text"
            onChange={handleInput}
            name="PhoneNumber"
            placeholder="Phone Number"
            value={form?.PhoneNumber || ""}
          />
        </div>

        <div>
          <label>Password*</label>
          <input
            className="border border-gray-300 rounded-lg p-2 w-full focus:outline-none focus:ring-2 focus:ring-blue-500 mt-2"
            type="password"
            onChange={handleInput}
            name="Password"
            placeholder="Password"
            value={form?.Password || ""}
          />
        </div>

        <div>
          <label>Confirm password*</label>
          <input
            className="border border-gray-300 rounded-lg p-2 w-full focus:outline-none focus:ring-2 focus:ring-blue-500 mt-2"
            type="password"
            onClick={handleInput}
            name="ConfirmPassword"
            placeholder="Confirm Password"
            value={form?.ConfirmPassword || ""}
          />
        </div>

        <div className="flex items-center">
          <input
            type="checkbox"
            className="mr-2"
            checked={agree}
            onClick={() => setAgree((a) => !a)}
          />
          <label
            className="text-sm text-gray-600 cursor-pointer"
            onClick={() => setAgree((a) => !a)}
          >
            I agree to the terms and conditions
          </label>
        </div>
        <button
          className="w-full bg-blue-500 text-white rounded-lg p-2 hover:bg-blue-600 transition duration-200"
          onClick={() => console.log(form)}
        >
          Sign Up
        </button>
        <div className="flex justify-center items-center flex-col">
          <div>Already have an account?</div>
          <a href="/signin">Sign In</a>
        </div>
      </div>
    </div>
  );
}
