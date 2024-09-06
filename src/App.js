import "./App.css";
import { Route, Router, Routes } from "react-router-dom";
import GuestRoutes from "./routes/GuestRoutes";
import AdminRoutes from "./routes/AdminRoutes";
import { AuthProvider } from "./setting/AuthContext";
import { RoleProvider } from "./setting/RoleContext";
import PartnerRoutes from "./routes/PartnerRoutes";

function App() {
  return (
    <div className="App">
      <AuthProvider>
        <RoleProvider>
          <Routes>
            <Route path="/*" element={<GuestRoutes />} />
            <Route path="/admin/*" element={<AdminRoutes />} />
            <Route path="/partner/*" element={<PartnerRoutes />} />
          </Routes>
        </RoleProvider>
      </AuthProvider>
    </div>
  );
}

export default App;
