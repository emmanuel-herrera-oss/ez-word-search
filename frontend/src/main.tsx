import { createRoot } from 'react-dom/client'
import { createHashRouter, Navigate, RouterProvider } from 'react-router-dom'

import '@mantine/core/styles.css';
import { MantineProvider } from '@mantine/core'

import { Root } from './pages/root/Root'
import { Home } from './pages/home/Home'
import { Profile } from './pages/profile/Profile.tsx'
import { Match } from './pages/match/Match.tsx';
import { Leaderboard } from './pages/leaderboard/Leaderboard.tsx';
import { History } from './pages/history/History.tsx';

import './index.css'
import { useState } from 'react';
import { DefaultPlayerProfile, PlayerProfile, PlayerProfileContext } from './components/PlayerProfileContext.tsx';

const router = createHashRouter([
  {
    path: '/',
    element: <Navigate to="/play/home"/>
  },
  {
    path: '/play',
    element: <Root></Root>,
    children: [
      {
        path: 'home',
        element: (<Home/>)
      },
      {
        path: 'profile',
        element: (<Profile/>)
      },
      {
        path: 'match',
        element: (<Match/>)
      },
      {
        path: 'leaderboard',
        element: (<Leaderboard/>)
      },
      {
        path: 'history',
        element: (<History/>)
      }
    ]
  }  
])

const App = () => {
  const [ playerProfile, setPlayerProfile ] = useState<PlayerProfile>(DefaultPlayerProfile)
  return(
  <>
    <PlayerProfileContext.Provider value={{ playerProfile, setPlayerProfile }}>
      <MantineProvider>
        <RouterProvider router={router} />
      </MantineProvider>
    </PlayerProfileContext.Provider>
  </>)
}

createRoot(document.getElementById('root')!).render(
  <>
    <App/>
  </>,
)
